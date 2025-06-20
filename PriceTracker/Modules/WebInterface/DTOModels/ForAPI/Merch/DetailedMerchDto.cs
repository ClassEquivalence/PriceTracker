


using PriceTracker.Core.Models.Domain;

namespace PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Merch
{
    public record DetailedMerchDto : BaseDomainWebInterfaceDto
    {
        public string Name { get; init; }
        public MerchPriceHistoryDto MerchPriceHistory { get; init; }
        public int ShopId { get; init; }

        public DetailedMerchDto(MerchPriceHistoryDto priceHistory, string name, int shopId,
            int merchId = default): base(merchId)
        {
            Name = name;
            MerchPriceHistory = priceHistory;
            ShopId = shopId;
        }
    }
}
