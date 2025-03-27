namespace PriceTracker.Models.BaseAppModels.PriceExtracting.ShopMerchExtracting
{
    public interface IShopMerchExtractor
    {
        public List<IShopMerch> GetAllShopMerches();
        public List<IShopMerch> GetNewShopMerches();
        public List<IShopMerch> GetKnownShopMerches();
    }
}
