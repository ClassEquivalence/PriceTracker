using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Facade;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Merch;
using PriceTracker.Modules.WebInterface.Mapping.Merch;

namespace PriceTracker.Modules.WebInterface.Services.MerchService
{
    public class MerchService
    {

        private readonly ILogger _logger;
        private readonly IShopRepositoryFacade _shopRepository;
        private readonly IMerchRepositoryFacade _merchRepository;
        private readonly IPriceHistoryRepositoryFacade _priceHistoryRepository;
        private readonly ITimestampedPriceRepositoryFacade _timestampedPriceRepository;
        private readonly ICitilinkMerchRepositoryFacade _citilinkMerchRepository;

        private readonly IDetailedMerchDtoMapper _detailedMerchDtoMapper;
        private readonly IOverviewMerchDtoMapper _overviewMerchDtoMapper;
        public MerchService(ILogger logger, IRepositoryFacade repository,
            IDetailedMerchDtoMapper detailedMerchDtoMapper,
            IOverviewMerchDtoMapper overviewMerchDtoMapper)
        {
            _logger = logger;
            _merchRepository = repository;
            _timestampedPriceRepository = repository;
            _priceHistoryRepository = repository;
            _shopRepository = repository;
            _citilinkMerchRepository = repository;

            _detailedMerchDtoMapper = detailedMerchDtoMapper;
            _overviewMerchDtoMapper = overviewMerchDtoMapper;
        }


        public IEnumerable<MerchOverviewDto> GetMerchesOfShop(int shopId)
        {
            return _merchRepository.Where(m => m.ShopId == shopId)
                .Select(_overviewMerchDtoMapper.Map);
        }


        public DetailedMerchDto? Get(int merchId)
        {
            var model = _merchRepository.GetModel(merchId);
            return model == null ? null : _detailedMerchDtoMapper.Map(model);
        }


        public bool Post(int shopId, MerchOverviewDto merch)
        {
            var shop = _shopRepository.GetModel(shopId);
            if (shop == null)
                return false;

            TimestampedPriceDto currentPrice = new(default, merch.CurrentPrice,
                DateTime.Now, default);
            MerchPriceHistoryDto priceHistoryDto = new(default, [],
                currentPrice, default);
            MerchDto merchDto = new(merch.Id, merch.Name, priceHistoryDto, shopId, default);

            _merchRepository.Create(merchDto);
            return true;
        }


        public bool Put(int merchId, string name)
        {
            var model = _merchRepository.GetModel(merchId);
            if (model == null)
                return false;

            MerchDto updated = new(model.Id, name, model.PriceTrack, model.ShopId,
                model.PriceHistoryId);
            return _merchRepository.Update(updated);
        }

        public bool Delete(int merchId)
        {
            return _merchRepository.Delete(merchId);
        }

        public bool PostPrice(int merchId, TimestampedPriceDto timestampedPrice)
        {
            var priceHistory = getPriceHistoryByMerch(merchId);
            if (priceHistory == null)
                return false;

            priceHistory.PreviousTimestampedPricesList.Add(timestampedPrice);
            return _priceHistoryRepository.Update(priceHistory);

        }

        public bool SetCurrentPrice(int merchId, decimal currentPrice)
        {
            var priceHistory = getPriceHistoryByMerch(merchId);
            if (priceHistory == null)
                return false;

            List<TimestampedPriceDto> previousPrices = priceHistory.PreviousTimestampedPricesList;
            previousPrices.Add(priceHistory.CurrentPrice);

            TimestampedPriceDto updatedCurrentPrice = new(default, currentPrice,
                DateTime.Now, default);
            MerchPriceHistoryDto updated = new(priceHistory.Id, previousPrices,
                updatedCurrentPrice, priceHistory.MerchId);

            return _priceHistoryRepository.Update(updated);

        }


        public bool RemoveTimestampedPrice(int timestampedPriceId)
        {
            var timestampedPrice = _timestampedPriceRepository.GetModel(timestampedPriceId);
            if (timestampedPrice == null)
                return false;
            var priceHistory = _priceHistoryRepository.GetModel(timestampedPrice.MerchPriceHistoryId);
            if (priceHistory == null)
                return false;

            // В строке ниже CurrentPrice, по задуманной логике, удалить нельзя. Можно только
            // одну из прежних.
            return priceHistory.PreviousTimestampedPricesList.Remove(timestampedPrice);

        }

        public bool ClearOldPrices(int merchId)
        {
            var priceHistory = getPriceHistoryByMerch(merchId);
            if (priceHistory == null)
                return false;
            priceHistory.PreviousTimestampedPricesList.Clear();

            return _priceHistoryRepository.Update(priceHistory);
        }


        public DetailedMerchDto? GetCitilinkMerch(string citilinkMerchCode)
        {
            bool isGotten = _citilinkMerchRepository.TryGetSingleByCitilinkId
                (citilinkMerchCode, out var dto);

            if (!isGotten || dto == null)
                return null;
            else
                return _detailedMerchDtoMapper.Map(dto);
        }

        private MerchPriceHistoryDto? getPriceHistoryByMerch(int merchId)
        {
            var priceHistory = _priceHistoryRepository.Where(ph => ph.MerchId == merchId).SingleOrDefault();
            return priceHistory;
        }
    }
}
