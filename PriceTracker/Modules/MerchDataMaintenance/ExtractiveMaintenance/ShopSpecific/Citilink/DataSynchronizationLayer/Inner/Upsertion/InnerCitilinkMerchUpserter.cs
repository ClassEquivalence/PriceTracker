using PriceTracker.Core.CommonModels.Domain;
using PriceTracker.Core.CommonModels.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;
using System.Text;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Upsertion
{
    public class InnerCitilinkMerchUpserter : IInnerCitilinkMerchUpserter
    {
        private readonly IMerchDataMaintenanceRepositoryAdapter _merchRepository;
        private readonly ILogger? _logger;
        List<CitilinkMerchParsingDto> queuedData = [];
        private readonly int _upsertablePortionSize;
        public event Action? MerchPortionUpserted;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private bool isCompleted = false;

        public InnerCitilinkMerchUpserter(IMerchDataMaintenanceRepositoryAdapter repository,
            int upsertablePortionSize = 100, ILogger? logger = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(upsertablePortionSize,
                $"{nameof(InnerCitilinkMerchUpserter)}: {nameof(upsertablePortionSize)} " +
                $"должен быть больше 0.");
            _merchRepository = repository;

            _logger = logger;
            _upsertablePortionSize = upsertablePortionSize;
        }

        public async Task EnqueueForUpsertionAsync(CitilinkMerchParsingDto dto)
        {
            EnsureActive();
            queuedData.Add(dto);

            if (queuedData.Count >= _upsertablePortionSize)
            {
                await FlushAsync();
            }
        }

        public async Task CompleteAsync()
        {
            EnsureActive();
            await FlushAsync();
            isCompleted = true;
        }

        private async Task FlushAsync()
        {
            _logger?.LogTrace($"{nameof(FlushAsync)}: Merches are flushed");
            await _semaphore.WaitAsync();
            if (!(queuedData.Count > 0))
                return;

            await UpsertBundle(queuedData);
            queuedData.Clear();

            _semaphore.Release();
        }

        private async Task UpsertBundle(List<CitilinkMerchParsingDto> citilinkMerchesDataBundle)
        {
            _logger?.LogTrace($"{nameof(UpsertBundle)}: Merches bundle starts getting upserted.");
            /*
             Пайплайн работы апсершна:
            - На вход принимается список спаршенных dto'шек.
            - Список разбивается на 2 части по критерию: обновляем/добавляем.
            
            - Обновляем-часть сопоставляется с найденными в базе товарами
            - Найденные в базе товары обновляются
            
            - Добавляем-часть маппится в семантику найденных в базе товаров;
            - Добавляем-часть добавляется.
             */


            // Обращаемся к репозиторию для получения тех товаров, у которых есть
            // CitilinkId (тобишь таких товаров которые в базе уже есть и их надо обновить)
            // из представленных в dto товаров.

            var upsertableCitilinkMerchesIds = citilinkMerchesDataBundle.Select(m => m.CitilinkId).ToArray();
            var merchesInDb = _merchRepository.GetMultiple(upsertableCitilinkMerchesIds);
            var databasesCitilinkMerchesIds = merchesInDb.Select(m => m.CitilinkId);

            // Список на обновлени - то, что в БД имеется.
            List<CitilinkMerchParsingDto> toUpdate = citilinkMerchesDataBundle.Where
                (dto => databasesCitilinkMerchesIds.Contains(dto.CitilinkId)).ToList();
            // Список на вставку - то, чего в БД нет.
            List<CitilinkMerchParsingDto> toInsert = citilinkMerchesDataBundle.Where
                (dto => !databasesCitilinkMerchesIds.Contains(dto.CitilinkId)
                && !toUpdate.Contains(dto)).ToList();

            // Убедимся что повторений в списках нет. Ситилинк может возвращать повторы.
            toInsert = toInsert.DistinctBy(dto => dto.CitilinkId).ToList();
            toUpdate = toUpdate.DistinctBy(dto => dto.CitilinkId).ToList();

            // Подготовим список на обновление.
            List<(CitilinkMerchParsingDto parsingDto,
            CitilinkMerchDto dbDto)> merchDtoPairsToUpdate = [];
            foreach (var updateable in toUpdate)
            {
                var coreDtoToUpdate = merchesInDb.Single(m => m.CitilinkId == updateable.CitilinkId);
                var updated = UpdateSingleDto(coreDtoToUpdate, updateable.Price);
                merchDtoPairsToUpdate.Add((updateable, coreDtoToUpdate));
            }

            // Апсертим товары по разделенным спискам.
            var updTask = UpdateBundle(merchDtoPairsToUpdate);
            var insertTask = InsertBundle(toInsert);
            await updTask;
            await insertTask;

            _logger?.LogTrace($"{nameof(InnerCitilinkMerchUpserter)}: Порция товаров заапсершенна.");
        }

        // Требования ко входу: совпадение CitilinkId (к-рый string). Уникальность CitilinkId 
        // в рамках разных элементов списка.
        private async Task UpdateBundle(List<(CitilinkMerchParsingDto parsingDto,
            CitilinkMerchDto dbDto)> merchDtoPairsToUpdate)
        {
            _logger?.LogTrace($"{nameof(UpdateBundle)}: Merch bundle's being updated");

            List<CitilinkMerchDto> coreDtosToUpdate = [];

            foreach (var updateable in merchDtoPairsToUpdate)
            {
                var newPrice = updateable.parsingDto.Price;
                var updated = UpdateSingleDto(updateable.dbDto, newPrice);
                coreDtosToUpdate.Add(updated);
            }
            var updateTask = _merchRepository.UpdateManyAsync(coreDtosToUpdate);
            await updateTask;

            // Логгирование
            StringBuilder updatedMerchesSb = new();
            updatedMerchesSb.Append($"{nameof(InnerCitilinkMerchUpserter)}: товары с" +
                $" CitilinkId = ");
            foreach (var updateable in coreDtosToUpdate)
            {
                updatedMerchesSb.Append($"{updateable.CitilinkId}; ");
            }
            updatedMerchesSb.Append("в базу уже занесены, и были обновлены.");
            _logger?.LogTrace(updatedMerchesSb.ToString());
        }

        private async Task InsertBundle(List<CitilinkMerchParsingDto> insertableParsingDtos)
        {
            _logger?.LogTrace($"{nameof(UpdateBundle)}: Merch bundle's being inserted");

            List<CitilinkMerchDto> coreDtosToInsert = [];

            // Почему я маппинг оставил здесь?
            foreach (var insertable in insertableParsingDtos)
            {
                var currentPrice = new TimestampedPriceDto(default, insertable.Price,
                    DateTime.UtcNow, default);
                var priceHistory = new MerchPriceHistoryDto(default, [], currentPrice, default);
                var inserted = new CitilinkMerchDto(default, insertable.Name,
                    priceHistory, insertable.CitilinkId, default);
                coreDtosToInsert.Add(inserted);
            }
            var insertTask = _merchRepository.CreateManyAsync(coreDtosToInsert);
            await insertTask;

            // Логгирование
            StringBuilder insertedMerchesSb = new();
            insertedMerchesSb.Append($"{nameof(InnerCitilinkMerchUpserter)}: товаров с" +
                $" CitilinkId = ");
            foreach (var insertable in coreDtosToInsert)
            {
                insertedMerchesSb.Append($"{insertable.CitilinkId}; ");
            }
            insertedMerchesSb.Append("в базе не было, но они добавились.");
            _logger?.LogTrace(insertedMerchesSb.ToString());
        }

        private CitilinkMerchDto UpdateSingleDto(CitilinkMerchDto dto, decimal currentPrice)
        {
            MerchPriceHistoryDto oldPriceHistory = dto.PriceTrack;
            TimestampedPriceDto currentPriceDto = new(default, currentPrice, DateTime.Now,
                oldPriceHistory.Id);
            MerchPriceHistoryDto updatedPriceHistory = oldPriceHistory with
            {
                PreviousTimestampedPricesList =
                oldPriceHistory.PreviousTimestampedPricesList.Append(oldPriceHistory.CurrentPrice)
                .ToList(),
                CurrentPrice = currentPriceDto
            };
            var updatedCoreDto = dto with { PriceTrack = updatedPriceHistory };
            return updatedCoreDto;
        }

        private void EnsureActive()
        {
            if (isCompleted)
                throw new InvalidOperationException($"{nameof(InnerCitilinkMerchUpserter)} - завершен." +
                    $" Вызов метода отклонен.");
        }
    }
}


