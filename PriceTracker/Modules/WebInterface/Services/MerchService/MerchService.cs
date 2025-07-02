using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Facade;



namespace PriceTracker.Modules.WebInterface.Services.MerchService
{
    public class MerchService : IMerchService
    {

        private readonly ILogger _logger;
        private readonly IMerchRepositoryFacade _merchRepository;
        private readonly IPriceHistoryRepositoryFacade _priceHistoryRepository;
        private readonly ITimestampedPriceRepositoryFacade _timestampedPriceRepository;

        public List<MerchDto> Merches => _merchRepository.
            Where(m => true);

        public MerchService(ILogger<Program> logger, IRepositoryFacade repository)
        {
            _logger = logger;
            _merchRepository = repository;
            _timestampedPriceRepository = repository;
            _priceHistoryRepository = repository;
        }

        public List<MerchDto> GetMerchesOfShop(int shopId)
        {
            return _merchRepository.Where(m => m.ShopId == shopId);
        }
        public MerchDto? GetMerch(int merchId)
        {
            return _merchRepository.GetModel(merchId);
        }

        // TODO: [Валидация] добавить проверку уникальности товара.
        // впрочем, это потом - когда появятся уникальные признаки товара
        // в рамках магазина.
        public virtual bool TryCreate(MerchDto merch)
        {
            _merchRepository.Create(merch);
            return true;
        }
        public bool TryDelete(int merchId)
        {
            return _merchRepository.Delete(merchId);
        }
        public bool TryChangeName(int merchId, string newName)
        {
            var model = _merchRepository.GetModel(merchId);
            if (model == null) return false;

            MerchDto updatedModel = new(model.Id, newName, model.PriceTrack,
                model.ShopId, model.PriceHistoryId);

            return _merchRepository.Update(updatedModel);
        }

        public bool TryAddTimestampedPrice(int merchId, TimestampedPriceDto timestampedPrice)
        {
            var model = _merchRepository.GetModel(merchId);
            if (model == null) return false;
            model.PriceTrack.PreviousTimestampedPricesList.Add(timestampedPrice);
            return _merchRepository.Update(model);
        }
        public bool SetCurrentPrice(int merchId, decimal currentPrice)
        {
            var model = _merchRepository.GetModel(merchId);
            if (model == null) return false;

            var priceTrack = model.PriceTrack;
            var previousPrices = priceTrack.PreviousTimestampedPricesList.
                Append(priceTrack.CurrentPrice).ToList();
            var currentTimestampedPrice = new TimestampedPriceDto(default, currentPrice,
                DateTime.Now, priceTrack.Id);

            var updatedPriceTrack = new MerchPriceHistoryDto(priceTrack.Id,
                previousPrices, currentTimestampedPrice, priceTrack.MerchId);

            return _priceHistoryRepository.Update(updatedPriceTrack);


        }

        public bool RemoveSingleTimestampedPrice(int timestampedPriceId)
        {
            return _timestampedPriceRepository.Delete(timestampedPriceId);
        }
        public bool ClearOldPrices(int merchId)
        {
            var merch = _merchRepository.GetModel(merchId);
            if (merch == null) return false;
            var priceTrack = merch.PriceTrack;

            MerchPriceHistoryDto updatedPriceTrack = new(priceTrack.Id, [],
                priceTrack.CurrentPrice, priceTrack.MerchId);

            return _priceHistoryRepository.Update(updatedPriceTrack);
        }

    }
}
