namespace PriceTracker.Modules.Repository.Entities.Domain.MerchPriceHistory
{
    public class CurrentPricePointerEntity : BaseEntity
    {

        public int MerchPriceHistoryId { get; set; }
        public MerchPriceHistoryEntity MerchPriceHistory { get; set; }
        public int CurrentPriceId { get; set; }
        public TimestampedPriceEntity CurrentPrice { get; set; }


        public CurrentPricePointerEntity(int merchPriceHistoryId,
            int currentPriceId, int Id) : base(Id)
        {
            MerchPriceHistoryId = merchPriceHistoryId;
            CurrentPriceId = currentPriceId;
        }


    }
}
