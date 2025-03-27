namespace PriceTracker.Models.BaseAppModels
{
    public class ShopMerchPriceTrack
    {
        public ShopMerchPriceTrack(TimedPrice price) 
        {
            TimedPrices = [price];
        }
        public List<TimedPrice> TimedPrices { get; set; }
        public void AddPrice(TimedPrice timedPrice)
        {
            TimedPrices.Add(timedPrice);
        }
    }
}
