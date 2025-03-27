namespace PriceTracker.Models.BaseAppModels.PriceExtracting
{
    public interface IShopPriceExtractor
    {
        public TimedPrice Extract(IShopMerch merch);
    }
}
