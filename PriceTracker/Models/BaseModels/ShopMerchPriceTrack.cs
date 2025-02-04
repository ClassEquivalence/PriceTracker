namespace PriceTracker.Models.BaseModels
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
