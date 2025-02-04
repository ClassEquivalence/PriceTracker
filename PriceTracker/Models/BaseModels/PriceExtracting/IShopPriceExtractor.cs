namespace PriceTracker.Models.BaseModels.PriceExtracting
{
    public interface IShopPriceExtractor
    {
        public TimedPrice Extract(IShopMerch merch);
    }
}
