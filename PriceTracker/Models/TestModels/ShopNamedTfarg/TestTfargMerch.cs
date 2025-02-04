using PriceTracker.Models.BaseModels;

namespace PriceTracker.Models.TestModels.ShopNamedTfarg
{
    public class TestTfargMerch: IShopMerch
    {
        public TestTfargMerch(string name, decimal currentPrice) 
        {
            Name = name;
            CurrentPrice = TimedPrice.CreateCurrentPrice(currentPrice);
            PriceTrack = new(CurrentPrice);
        }
        public string Name { get; set; }
        public int Id { get; set; }
        public ShopMerchPriceTrack PriceTrack { get; set; }
        public TimedPrice CurrentPrice { get; set; }
    }
}
