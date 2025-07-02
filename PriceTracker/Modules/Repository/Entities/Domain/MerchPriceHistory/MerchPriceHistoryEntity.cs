namespace PriceTracker.Modules.Repository.Entities.Domain.MerchPriceHistory
{
    public class MerchPriceHistoryEntity : BaseEntity
    {
        public List<TimestampedPriceEntity> TimestampedPrices { get; set; }
        public CurrentPricePointerEntity? CurrentPricePointer { get; set; }

        public MerchEntity Merch { get; set; }
        public int MerchId { get; set; }

        // Исправить ситуацию с non nullable.
        public MerchPriceHistoryEntity(int id = default) : base(id)
        {
        }

    }
}
