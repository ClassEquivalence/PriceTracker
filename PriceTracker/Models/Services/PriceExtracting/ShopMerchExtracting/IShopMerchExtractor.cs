using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.Services.PriceExtracting.ShopMerchExtracting
{
    public interface IShopMerchExtractor
    {
        public List<IMerchModel> GetAllShopMerches();
        public List<IMerchModel> GetNewShopMerches();
        public List<IMerchModel> GetKnownShopMerches();
    }
}
