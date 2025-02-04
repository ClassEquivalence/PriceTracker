namespace PriceTracker.Models.BaseModels
{
    public struct TimedPrice
    {
        public decimal Price { get; set; }
        public DateTime DateTime { get; set; }
        public static TimedPrice CreateCurrentPrice(decimal price)
        {
            TimedPrice timedPrice = new TimedPrice {Price = price, DateTime = DateTime.Now};
            return timedPrice;
        }
    }
}
