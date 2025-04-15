namespace PriceTracker.Models.DomainModels
{
    public interface IMerchModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MerchPriceHistory PriceTrack { get; init; }
        public TimestampedPrice CurrentPrice { get; }
    }
}
