using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models.Domain;
using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Core.Models.Infrastructure;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.Repository.Mapping.Domain;
using PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel.MerchRepository;
using PriceTracker.Modules.Repository.Repositories.Domain.EntityLevel;
using PriceTracker.Modules.Repository.Repositories.Process;
using PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink;

namespace PriceTracker.Modules.Repository.Facade
{

    // TODO: Составляющая с TimeExtractionHappened - костыльная.
    // Составляющая с CitilinkParsingExecutionStateRepository - тоже.

    public class RepositoryFacade : IRepositoryFacade
    {
        private readonly LastTimeExtractionHappenedRepository _timeExtractionHappenedRepository;
        private readonly CitilinkExtractionStateRepository
            _citilinkParsingExecutionStateRepository;
        private readonly CitilinkExtractorStorageStateRepository
            _citilinkExtractorStorageStateRepository;


        private readonly CitilinkMerchRepository _citilinkMerchRepository;
        private readonly ShopRepository _shopRepository;
        private readonly MerchRepository _merchRepository;
        private readonly PriceHistoryRepository _priceHistoryRepository;
        private readonly TimestampedPriceRepository _timestampedPriceRepository;

        private readonly ILogger? _logger;


        public RepositoryFacade(PriceTrackerContext dbContext,
            IDbContextFactory<PriceTrackerContext> dbcontextFactory, ILogger<Program>? logger = null)
        {
            _logger = logger;

            _timeExtractionHappenedRepository = new(dbContext);

            CitilinkCatalogUrlBranchMapper citilinkCatalogUrlBranchMapper = new(logger);
            CitilinkCatalogUrlTreeMapper citilinkUrlTreeMapper = new(citilinkCatalogUrlBranchMapper);
            CitilinkExtractionStateMapper citilinkExtractionStateMapper = new(citilinkUrlTreeMapper);
            _citilinkParsingExecutionStateRepository = new(dbcontextFactory, citilinkExtractionStateMapper);

            CitilinkExtractorStorageStateMapper citilinkExtractorStorageStateMapper = new();
            _citilinkExtractorStorageStateRepository = new(dbcontextFactory, 
                citilinkExtractorStorageStateMapper);

            TimestampedPriceEntityRepository timestampedPriceEntityRepository
                = new(dbcontextFactory);
            TimestampedPriceMapper timestampedPriceMapper = new();

            PriceHistoryEntityRepository priceHistoryEntityRepository = new(dbcontextFactory);
            PriceHistoryMapper priceHistoryMapper = new(timestampedPriceMapper, logger);


            CitilinkMerchEntityRepository citilinkMerchEntityRepository = new(dbcontextFactory, logger);
            CitilinkMerchCoreToEntityMapper citilinkMerchMapper = new(priceHistoryMapper);

            // TODO: При добавлении новых типов товаров (наследников Merch), пересмотреть инициализацию маппера,
            // да и сам маппер.
            MerchMapper merchMapper = new(priceHistoryMapper);

            ShopEntityRepository shopEntityRepository = new(dbcontextFactory);
            ShopCoreToEntityMapper shopMapper = new(merchMapper);

            _citilinkMerchRepository = new CitilinkMerchRepository(citilinkMerchEntityRepository,
                citilinkMerchMapper, logger);
            _shopRepository = new(shopEntityRepository, shopMapper);

            CitilinkMerchRepositoryAdapter citilinkMerchRepositoryAdapter = new(_citilinkMerchRepository);
            _merchRepository = new([citilinkMerchRepositoryAdapter]);
            _priceHistoryRepository = new(priceHistoryEntityRepository,
                priceHistoryMapper);
            _timestampedPriceRepository = new(timestampedPriceEntityRepository,
                timestampedPriceMapper);
        }

        public bool Any(Func<MerchDto, bool> predicate)
        {
            return _merchRepository.Any(predicate);
        }

        public bool Any(Func<ShopDto, bool> predicate)
        {
            return _shopRepository.Any(predicate);
        }

        public bool Any(Func<MerchPriceHistoryDto, bool> predicate)
        {
            return _priceHistoryRepository.Any(predicate);
        }

        public bool Any(Func<TimestampedPriceDto, bool> predicate)
        {
            return _timestampedPriceRepository.Any(predicate);
        }

        public void Create(MerchDto entity)
        {
            _merchRepository.Create(entity);
        }

        public void Create(ShopDto entity)
        {
            _shopRepository.Create(entity);
        }

        public void Create(MerchPriceHistoryDto entity)
        {
            _priceHistoryRepository.Create(entity);
        }

        public void Create(TimestampedPriceDto entity)
        {
            _timestampedPriceRepository.Create(entity);
        }



        public List<MerchDto> GetAll()
        {
            return _merchRepository.GetAll();
        }

        public ShopDto GetCitilinkShop()
        {
            return _shopRepository.SingleOrDefault(shop => shop.Name == RepositoryParameters.CitilinkShopName)
                ?? throw new InvalidOperationException("Магазин Citilink не инициализирован.");
        }

        public (DateTime lastTimeStart, DateTime lastTimeFinish) GetLastTimeExtractionProcessHappened()
        {
            return _timeExtractionHappenedRepository.GetLastTimeExtractionProcessHappened();
        }

        public MerchDto? GetModel(int id)
        {
            return _merchRepository.GetModel(id);
        }

        public CitilinkExtractionStateDto? Provide()
        {
            return _citilinkParsingExecutionStateRepository.Get();
        }

        public void Save(CitilinkExtractionStateDto info)
        {
            _citilinkParsingExecutionStateRepository.Set(info);
        }


        public void SetFinishTimeExtractionProcessHappened(DateTime time)
        {
            _timeExtractionHappenedRepository.SetFinishTimeExtractionProcessHappened(time);
        }

        public void SetStartTimeExtractionProcessHappened(DateTime time)
        {
            _timeExtractionHappenedRepository.SetStartTimeExtractionProcessHappened(time);
        }

        public MerchDto? SingleOrDefault(Func<MerchDto, bool> predicate)
        {
            return _merchRepository.SingleOrDefault(predicate);
        }

        public ShopDto? SingleOrDefault(Func<ShopDto, bool> predicate)
        {
            return _shopRepository.SingleOrDefault(predicate);
        }

        public MerchPriceHistoryDto? SingleOrDefault(Func<MerchPriceHistoryDto, bool> predicate)
        {
            return _priceHistoryRepository.SingleOrDefault(predicate);
        }

        public TimestampedPriceDto? SingleOrDefault(Func<TimestampedPriceDto, bool> predicate)
        {
            return _timestampedPriceRepository.SingleOrDefault(predicate);
        }

        public bool TryGetSingleByCitilinkId(string citilinkId, out CitilinkMerchDto? dto)
        {
            dto = _citilinkMerchRepository.SingleOrDefault(cm => cm.CitilinkId == citilinkId);
            return !(dto == null);
        }

        public bool TryInsert(CitilinkMerchDto dto)
        {
            _citilinkMerchRepository.Create(dto);
            return true;
        }

        public bool TryUpdate(CitilinkMerchDto dto)
        {
            return _citilinkMerchRepository.Update(dto);
        }

        public bool Update(MerchDto entity)
        {
            return _merchRepository.Update(entity);
        }

        public bool Update(ShopDto entity)
        {
            return _shopRepository.Update(entity);
        }

        public bool Update(MerchPriceHistoryDto entity)
        {
            return _priceHistoryRepository.Update(entity);
        }

        public bool Update(TimestampedPriceDto entity)
        {
            return _timestampedPriceRepository.Update(entity);
        }

        public List<CitilinkMerchDto> Where(Func<CitilinkMerchDto, bool> selector)
        {
            return _citilinkMerchRepository.Where(selector);
        }

        public List<MerchDto> Where(Func<MerchDto, bool> predicate)
        {
            return _merchRepository.Where(predicate);
        }

        public List<ShopDto> Where(Func<ShopDto, bool> predicate)
        {
            return _shopRepository.Where(predicate);
        }

        public List<MerchPriceHistoryDto> Where(Func<MerchPriceHistoryDto, bool> predicate)
        {
            return _priceHistoryRepository.Where(predicate);
        }

        public List<TimestampedPriceDto> Where(Func<TimestampedPriceDto, bool> predicate)
        {
            return _timestampedPriceRepository.Where(predicate);
        }

        bool IDomainRepositoryFacade<MerchDto>.Delete(int id)
        {
            return _merchRepository.Delete(id);
        }

        bool IDomainRepositoryFacade<ShopDto>.Delete(int id)
        {
            return _shopRepository.Delete(id);
        }

        bool IDomainRepositoryFacade<MerchPriceHistoryDto>.Delete(int id)
        {
            return _priceHistoryRepository.Delete(id);
        }

        bool IDomainRepositoryFacade<TimestampedPriceDto>.Delete(int id)
        {
            return _timestampedPriceRepository.Delete(id);
        }

        List<ShopDto> IDomainRepositoryFacade<ShopDto>.GetAll()
        {
            return _shopRepository.GetAll();
        }

        List<MerchPriceHistoryDto> IDomainRepositoryFacade<MerchPriceHistoryDto>.GetAll()
        {
            return _priceHistoryRepository.GetAll();
        }

        List<TimestampedPriceDto> IDomainRepositoryFacade<TimestampedPriceDto>.GetAll()
        {
            return _timestampedPriceRepository.GetAll();
        }

        ShopDto? IDomainRepositoryFacade<ShopDto>.GetModel(int id)
        {
            return _shopRepository.GetModel(id);
        }

        MerchPriceHistoryDto? IDomainRepositoryFacade<MerchPriceHistoryDto>.GetModel(int id)
        {
            return _priceHistoryRepository.GetModel(id);
        }

        TimestampedPriceDto? IDomainRepositoryFacade<TimestampedPriceDto>.GetModel(int id)
        {
            return _timestampedPriceRepository.GetModel(id);
        }

        void ICitilinkMiscellaneousRepositoryFacade.
            SetExtractorStorageState(CitilinkExtractorStorageStateDto storageStateDto)
        {
            _citilinkExtractorStorageStateRepository.
                Set(storageStateDto);
        }

        CitilinkExtractorStorageStateDto? ICitilinkMiscellaneousRepositoryFacade.
            GetExtractorStorageState()
        {
            return _citilinkExtractorStorageStateRepository.
                Get();
        }

        public void EnsureRepositoryInitialized()
        {
            RepositoryInitializer initializer = new();
            initializer.EnsureInitialized(_shopRepository);
        }

        async Task ICitilinkMerchRepositoryFacade.CreateManyAsync(List<CitilinkMerchDto> citilinkMerches)
        {
            _logger?.LogTrace($"{nameof(RepositoryFacade)}, {nameof(ICitilinkMerchRepositoryFacade.CreateManyAsync)}: метод вызван.");
            await _citilinkMerchRepository.CreateManyAsync(citilinkMerches);
        }

        List<CitilinkMerchDto> ICitilinkMerchRepositoryFacade.GetMultiple(string[] citilinkIds)
        {
            return _citilinkMerchRepository.Where(cm => citilinkIds.Contains(cm.CitilinkId)).ToList();
        }

        async Task ICitilinkMerchRepositoryFacade.UpdateManyAsync(List<CitilinkMerchDto> citilinkMerches)
        {
            _logger?.LogTrace($"{nameof(RepositoryFacade)}, {nameof(ICitilinkMerchRepositoryFacade.UpdateManyAsync)}: метод вызван.");
            await _citilinkMerchRepository.UpdateManyAsync(citilinkMerches);
        }
    }
}
