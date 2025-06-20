using PriceTracker.Core.Models.Domain;
using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Mapping.Domain;
using PriceTracker.Modules.Repository.Mapping.ShopSpecific;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;
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
        private readonly CitilinkParsingExecutionStateRepository
            _citilinkParsingExecutionStateRepository;

        
        private readonly CitilinkMerchRepository _citilinkMerchRepository;
        private readonly ShopRepository _shopRepository;



        public RepositoryFacade(PriceTrackerContext dbContext)
        {
            _timeExtractionHappenedRepository = new(dbContext);
            _citilinkParsingExecutionStateRepository = new(dbContext);

            TimestampedPriceEntityRepository timestampedPriceEntityRepository
                = new(dbContext);
            TimestampedPriceMapper timestampedPriceMapper = new(dto=>timestampedPriceEntityRepository.
            GetEntity(dto.Id));

            PriceHistoryEntityRepository priceHistoryEntityRepository = new(dbContext);
            PriceHistoryMapper priceHistoryMapper = new(timestampedPriceMapper, 
                dto=>priceHistoryEntityRepository.GetEntity(dto.Id));

            CitilinkMerchEntityRepository citilinkMerchEntityRepository = new(dbContext);
            CitilinkMerchCoreToEntityMapper citilinkMerchMapper = new(priceHistoryMapper,
                dto => citilinkMerchEntityRepository.GetEntity(dto.Id));

            // TODO: При добавлении новых типов товаров (наследников Merch), пересмотреть инициализацию маппера,
            // да и сам маппер.
            MerchMapper merchMapper = new(dto => citilinkMerchEntityRepository.GetEntity(dto.Id),
                priceHistoryMapper);

            ShopEntityRepository shopEntityRepository = new(dbContext);
            ShopCoreToEntityMapper shopMapper = new(merchMapper, dto => shopEntityRepository.
            GetEntity(dto.Id));

            _citilinkMerchRepository = new CitilinkMerchRepository(citilinkMerchEntityRepository,
                citilinkMerchMapper);
            _shopRepository = new(shopEntityRepository, shopMapper);
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

        public CitilinkParsingExecutionState Provide()
        {
            return _citilinkParsingExecutionStateRepository.Provide();
        }

        public void Save(CitilinkParsingExecutionState info)
        {
            _citilinkParsingExecutionStateRepository.Save(info);
        }

        public void SetFinishTimeExtractionProcessHappened(DateTime time)
        {
            _timeExtractionHappenedRepository.SetFinishTimeExtractionProcessHappened(time);
        }

        public void SetStartTimeExtractionProcessHappened(DateTime time)
        {
            _timeExtractionHappenedRepository.SetStartTimeExtractionProcessHappened(time);
        }

        public bool TryGetSingleByCitilinkId(string citilinkId, out CitilinkMerchDto? dto)
        {
            dto = _citilinkMerchRepository.SingleOrDefault(cm=>cm.CitilinkId == citilinkId);
            return ! (dto == null);
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

        public List<CitilinkMerchDto> Where(Func<CitilinkMerchDto, bool> selector)
        {
            return _citilinkMerchRepository.Where(selector);
        }
    }
}
