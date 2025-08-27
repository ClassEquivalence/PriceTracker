using PriceTracker.Core.Models.Domain;
using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services;
using PriceTracker.Modules.Repository.Facade.Citilink;
using System.Text;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink
{
    public class CitilinkMerchDataUpserter :
        IMerchDataConsumer<CitilinkMerchParsingDto>
    {
        private readonly ICitilinkMerchRepositoryFacade _merchRepository;
        private readonly ShopDto _citilinkShop;

        private readonly ILogger? _logger;

        private readonly SemaphoreSlim _semaphore = new(1, 1);

        List<CitilinkMerchParsingDto> queuedData = [];

        private readonly int _upsertablePortionSize;

        public event Action? MerchPortionUpserted;

        public CitilinkMerchDataUpserter(ICitilinkMerchRepositoryFacade repository, ShopDto
            citilink, int upsertablePortionSize = 100, ILogger? logger = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(upsertablePortionSize,
                $"{nameof(CitilinkMerchDataUpserter)}: {nameof(upsertablePortionSize)} " +
                $"должен быть больше 0.");
            _merchRepository = repository;
            _citilinkShop = citilink;

            _logger = logger;
            _upsertablePortionSize = upsertablePortionSize;
        }


        ~CitilinkMerchDataUpserter()
        {
            _semaphore.Dispose();
        }


        /// <summary>
        /// Данный метод не должен исполняться в параллельных потоках, если вызывающий объект - один
        /// и тот же!
        /// </summary>
        /// <param name="complementaryData"></param>
        /// <returns></returns>
        public async Task Upsert(IAsyncEnumerable<CitilinkMerchParsingDto> complementaryData)
        {
            await _semaphore.WaitAsync();

            _logger?.LogTrace($"{nameof(CitilinkMerchDataUpserter)}: Upsertion инициирован.");
            // TODO: Хорошо бы вынести логику добавления информации, или раздробить, пересмотреть ответсвтенность
            // за неё.

            int i = 0;

            await foreach (var parsingDto in complementaryData)
            {
                queuedData.Add(parsingDto);
                i++;
                if (i % _upsertablePortionSize == 0)
                {
                    await UpsertBundle(queuedData);
                    queuedData.Clear();

                    MerchPortionUpserted?.Invoke();
                }
            }
            await UpsertBundle(queuedData);
            queuedData.Clear();

            _logger?.LogTrace($"{nameof(CitilinkMerchDataUpserter)}: Upsertion завершился.");

            _semaphore.Release();
        }


        private async Task UpsertBundle(List<CitilinkMerchParsingDto> citilinkMerchesBundle)
        {

            // Обращаемся к репозиторию для получения тех товаров, у которых есть
            // CitilinkId из представленных в dto товаров.
            var merchesInDb = _merchRepository.GetMultiple(citilinkMerchesBundle.Select
                (m => m.CitilinkId).ToArray());
            var citilinkIdsOfMerchesInDb = merchesInDb.Select(m => m.CitilinkId);

            List<CitilinkMerchParsingDto> toUpdate = citilinkMerchesBundle.Where
                (dto => citilinkIdsOfMerchesInDb.Contains(dto.CitilinkId)).ToList();

            List<CitilinkMerchParsingDto> toInsert = citilinkMerchesBundle.Where
                (dto => !citilinkIdsOfMerchesInDb.Contains(dto.CitilinkId)).ToList();

            List<CitilinkMerchDto> coreDtosToUpdate = [];
            //_merchRepository.GetMultiple
            //(toUpdate.Select(pdto=>pdto.CitilinkId).ToArray());
            List<CitilinkMerchDto> coreDtosToInsert = [];

            foreach (var updateable in toUpdate)
            {
                var coreDtoToUpdate = merchesInDb.Single(m => m.CitilinkId == updateable.CitilinkId);
                var updated = UpdateSingle(coreDtoToUpdate, updateable.Price);
                coreDtosToUpdate.Add(updated);
            }
            foreach (var insertable in toInsert)
            {
                var currentPrice = new TimestampedPriceDto(default, insertable.Price,
                    DateTime.UtcNow, default);
                var priceHistory = new MerchPriceHistoryDto(default, [], currentPrice, default);
                var inserted = new CitilinkMerchDto(default, insertable.Name,
                    priceHistory, insertable.CitilinkId, _citilinkShop.Id, default);
                coreDtosToInsert.Add(inserted);
            }
            var insertTask = _merchRepository.CreateManyAsync(coreDtosToInsert);
            var updateTask = _merchRepository.UpdateManyAsync(coreDtosToUpdate);
            await insertTask;
            await updateTask;


            StringBuilder insertedMerchesSb = new();
            insertedMerchesSb.Append($"{nameof(CitilinkMerchDataUpserter)}: товаров с" +
                $" CitilinkId = ");
            foreach (var insertable in coreDtosToInsert)
            {
                insertedMerchesSb.Append($"{insertable.CitilinkId}; ");
            }
            insertedMerchesSb.Append("в базе ещё нет. Сейчас они заходят в репозиторий.");


            StringBuilder updatedMerchesSb = new();
            updatedMerchesSb.Append($"{nameof(CitilinkMerchDataUpserter)}: товары с" +
                $" CitilinkId = ");
            foreach (var updateable in coreDtosToUpdate)
            {
                updatedMerchesSb.Append($"{updateable.CitilinkId}; ");
            }
            updatedMerchesSb.Append("в базу уже занесены.");


            _logger?.LogTrace(insertedMerchesSb.ToString());

            _logger?.LogTrace(updatedMerchesSb.ToString());

            _logger?.LogTrace($"{nameof(CitilinkMerchDataUpserter)}: Portion upserted.");
        }

        private CitilinkMerchDto UpdateSingle(CitilinkMerchDto dto, decimal currentPrice)
        {
            MerchPriceHistoryDto oldPriceHistory = dto.PriceTrack;

            TimestampedPriceDto currentPriceDto = new(default, currentPrice, DateTime.Now,
                oldPriceHistory.Id);

            MerchPriceHistoryDto priceHistory = new(oldPriceHistory.Id,
                oldPriceHistory.PreviousTimestampedPricesList.Append(oldPriceHistory.CurrentPrice)
                .ToList(), currentPriceDto, oldPriceHistory.MerchId);

            var updatedCoreDto = new CitilinkMerchDto(dto.Id, dto.Name, priceHistory,
                dto.CitilinkId, dto.ShopId, dto.PriceHistoryId);

            return updatedCoreDto;
        }

    }
}
