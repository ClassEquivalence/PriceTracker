using PriceTracker.Core.Models.Domain;
using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Facade;

namespace PriceTracker.Modules.WebInterface.Services.MerchService.Citilink
{
    public class CitilinkMerchService : ICitilinkMerchService
    {
        private readonly IMerchService _baseMerchService;
        private readonly ICitilinkMerchRepositoryFacade _repository;
        public CitilinkMerchService(IMerchService baseMerchService,
            ICitilinkMerchRepositoryFacade citilinkMerchRepository)
        {
            _baseMerchService = baseMerchService;
            _repository = citilinkMerchRepository;
        }

        public List<CitilinkMerchDto> Merches => throw new NotImplementedException();

        public bool ClearOldPrices(int merchId)
        {
            return _baseMerchService.ClearOldPrices(merchId);
        }

        public CitilinkMerchDto? GetByCitilinkId(string citilinkId)
        {
            bool gotten = _repository.TryGetSingleByCitilinkId(citilinkId, out var dto);
            return gotten ? dto : null;
        }

        public CitilinkMerchDto? GetMerch(int merchId)
        {
            return _repository.Where(m => m.Id == merchId).SingleOrDefault();
        }

        public List<CitilinkMerchDto> GetMerchesOfShop(int shopId)
        {
            return _repository.Where(m => m.ShopId == shopId);
        }


        public bool RemoveSingleTimestampedPrice(int timestampedPriceId)
        {
            return _baseMerchService.RemoveSingleTimestampedPrice(timestampedPriceId);
        }

        public bool SetCurrentPrice(int merchId, decimal currentPrice)
        {
            return _baseMerchService.SetCurrentPrice(merchId, currentPrice);
        }

        public bool TryAddTimestampedPrice(int merchId, TimestampedPriceDto timestampedPrice)
        {
            return _baseMerchService.TryAddTimestampedPrice(merchId, timestampedPrice);
        }

        public bool TryChangeName(int merchId, string newName)
        {
            return _baseMerchService.TryChangeName(merchId, newName);
        }

        public bool TryCreate(CitilinkMerchDto merch)
        {
            return _repository.TryInsert(merch);
        }

        public bool TryDelete(int merchId)
        {
            return _baseMerchService.TryDelete(merchId);
        }

    }
}
