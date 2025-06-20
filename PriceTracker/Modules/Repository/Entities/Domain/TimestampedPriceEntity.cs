namespace PriceTracker.Modules.Repository.Entities.Domain
{
    public class TimestampedPriceEntity : BaseEntity
    {
        public MerchPriceHistoryEntity MerchPriceHistory { get; set; }
        public int MerchPriceHistoryId { get; set; }
        public decimal Price { get; set; }
        public DateTime DateTime { get; set; }

        // разобраться с non nullable.
        public TimestampedPriceEntity(decimal price, DateTime dateTime, int id = default)
            : base(id)
        {
            Price = price;
            DateTime = dateTime;
        }
    }
}
