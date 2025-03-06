namespace PriceTracker.Models.BaseModels
{
    public interface IShopMerch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ShopMerchPriceTrack PriceTrack { get; set; }
        public TimedPrice CurrentPrice { get; set; }
    }
}
