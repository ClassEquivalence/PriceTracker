namespace PriceTracker.Models.BaseModels
{
    public interface IShopMerch
    {
        public string Name { get; set; }
        public ShopMerchPriceTrack PriceTrack { get; set; }
        public TimedPrice CurrentPrice { get; set; }
    }
}
