using PriceTracker.Models.DTOModels;

namespace PriceTracker.Models.DataAccess.Entities.Domain
{
    public class MerchPriceHistoryEntity : BaseEntity
    {
        public List<TimestampedPriceEntity> TimestampedPrices { get; set; }
        public TimestampedPriceEntity CurrentPrice { get; set; }
        public int CurrentPriceId { get; set; }

        public MerchEntity Merch { get; set; }
        public int MerchId { get; set; }

        // Исправить ситуацию с non nullable.
        public MerchPriceHistoryEntity(int id = default) : base(id)
        {
        }

    }
}
