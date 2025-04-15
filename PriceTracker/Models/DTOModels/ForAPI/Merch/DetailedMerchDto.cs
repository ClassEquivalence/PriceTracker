using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DTOModels.ForAPI.Merch
{
    public record DetailedMerchDto : BaseDTO
    {
        public string Name { get; init; }
        public MerchPriceHistoryDTO MerchPriceHistory { get; init; }
        public TimestampedPriceDTO CurrentPrice => MerchPriceHistory.CurrentPrice;
        public int ShopId { get; init; }

        public DetailedMerchDto(MerchPriceHistoryDTO priceHistory, string name, int shopId, 
            int merchId = default): base(merchId)
        {
            Name = name;
            MerchPriceHistory = priceHistory;
            ShopId = shopId;
        }
    }
}
