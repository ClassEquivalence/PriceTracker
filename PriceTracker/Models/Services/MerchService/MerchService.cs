using PriceTracker.Models.DataAccess.Entities;
using PriceTracker.Models.DataAccess.Repositories;
using PriceTracker.Models.DataAccess.Repositories.MerchRepository;
using PriceTracker.Models.DomainModels;
using System.Reflection.Metadata.Ecma335;

namespace PriceTracker.Models.Services.MerchService
{
    public class MerchService: IMerchService
    {
        private readonly ILogger _logger;
        private readonly MerchRepository _merchRepository;
        private readonly TimestampedPriceRepository _timestampedPriceRepository;
        public List<MerchModel> Merches => _merchRepository.GetAll();

        public MerchService(ILogger<Program> logger, MerchRepository merchRepository, 
            TimestampedPriceRepository timestampedPriceRepository)
        {
            _logger = logger;
            _merchRepository = merchRepository;
            _timestampedPriceRepository = timestampedPriceRepository;
        }

        public List<MerchModel> GetMerchesOfShop(int shopId)
        {
            return _merchRepository.Where(m => m.Shop.Id == shopId);
        }
        public MerchModel? GetMerch(int merchId)
        {
            return _merchRepository.GetModel(merchId);
        }

        // TODO: [Валидация] добавить проверку уникальности товара.
        // впрочем, это потом - когда появятся уникальные признаки товара
        // в рамках магазина.
        public virtual bool TryCreate(MerchModel merch)
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
            model.Name = newName;
            return _merchRepository.Update(model);
        }
        public bool TryAddTimestampedPrice(int merchId, TimestampedPrice timestampedPrice)
        {
            var model = _merchRepository.GetModel(merchId);
            if (model == null) return false;
            model.PriceTrack.AddHistoricalPrice(timestampedPrice);
            return true;
        }
        public bool SetCurrentPrice(int merchId, decimal currentPrice)
        {
            var model = _merchRepository.GetModel(merchId);
            if (model == null) return false;
            model.PriceTrack.CurrentPrice = TimestampedPrice.CreateCurrentPrice(currentPrice);
            return true;
        }

        public bool RemoveSingleTimestampedPrice(int timestampedPriceId)
        {
            return _timestampedPriceRepository.Delete(timestampedPriceId);
        }
        public bool ClearOldPrices(int merchId)
        {
            var merch = _merchRepository.GetModel(merchId);
            if (merch == null) return false;
            merch.PriceTrack.ClearHistoricalPrices();
            return true;
        }
    }
}
