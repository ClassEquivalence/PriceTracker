using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.Repository.Mapping.Domain;
using PriceTracker.Modules.Repository.Mapping.ShopSpecific;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel.MerchRepository;
using PriceTracker.Modules.Repository.Repositories.Domain.EntityLevel;
using PriceTracker.Modules.Repository.Repositories.Process;
using PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink;

namespace PriceTracker.Modules.Repository.Facade
{
    public class RepositoryFacadeProvider : IRepositoryFacadeProvider
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

        public RepositoryFacadeProvider(PriceTrackerContext dbContext,
            IDbContextFactory<PriceTrackerContext> dbcontextFactory, ILogger? logger = null)
        {
            _timeExtractionHappenedRepository = new(dbContext);
            _citilinkParsingExecutionStateRepository = new(dbContext);
            _citilinkExtractorStorageStateRepository = new(dbContext);

            TimestampedPriceEntityRepository timestampedPriceEntityRepository
                = new(dbcontextFactory);
            TimestampedPriceMapper timestampedPriceMapper = new(dto => timestampedPriceEntityRepository.
            GetEntity(dto.Id));

            PriceHistoryEntityRepository priceHistoryEntityRepository = new(dbcontextFactory);
            PriceHistoryMapper priceHistoryMapper = new(timestampedPriceMapper,
                dto => priceHistoryEntityRepository.GetEntity(dto.Id));


            CitilinkMerchEntityRepository citilinkMerchEntityRepository = new(dbcontextFactory);
            CitilinkMerchCoreToEntityMapper citilinkMerchMapper = new(priceHistoryMapper,
                dto => citilinkMerchEntityRepository.GetEntity(dto.Id));

            // TODO: При добавлении новых типов товаров (наследников Merch), пересмотреть инициализацию маппера,
            // да и сам маппер.
            MerchMapper merchMapper = new(dto => citilinkMerchEntityRepository.GetEntity(dto.Id),
                priceHistoryMapper);

            ShopEntityRepository shopEntityRepository = new(dbcontextFactory);
            ShopCoreToEntityMapper shopMapper = new(merchMapper, dto => shopEntityRepository.
            GetEntity(dto.Id));

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



        public ICitilinkMerchRepositoryFacade GetBufferedCitilinkMerchRepository()
        {
            throw new NotImplementedException();
        }

        public ICitilinkMerchRepositoryFacade GetCitilinkMerchRepository()
        {
            throw new NotImplementedException();
        }

        public ICitilinkMiscellaneousRepositoryFacade GetCitilinkMiscellaneousRepository()
        {
            throw new NotImplementedException();
        }

        public IPriceHistoryRepositoryFacade GetPriceHistoryRepository()
        {
            throw new NotImplementedException();
        }

        public IShopRepositoryFacade GetShopRepository()
        {
            throw new NotImplementedException();
        }

        public IShopSelectorFacade GetShopSelector()
        {
            throw new NotImplementedException();
        }

        public ITimestampedPriceRepositoryFacade GetTimestampedPriceRepository()
        {
            throw new NotImplementedException();
        }
    }
}
