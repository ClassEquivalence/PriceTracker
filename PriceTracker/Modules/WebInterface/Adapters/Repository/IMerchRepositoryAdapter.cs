using PriceTracker.Modules.WebInterface.API.DTOModels.Merch;

namespace PriceTracker.Modules.WebInterface.Adapters.Repository
{
    public interface IMerchRepositoryAdapter
    {
        public Task<DetailedMerchDto?> GetMerchAsync(int merchId);
        public Task<DetailedMerchDto?> GetCitilinkMerchAsync(string citilinkMerchCode);
    }
}
