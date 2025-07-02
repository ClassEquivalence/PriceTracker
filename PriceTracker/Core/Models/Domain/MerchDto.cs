
namespace PriceTracker.Core.Models.Domain
{
    public record MerchDto(int Id, string Name, MerchPriceHistoryDto PriceTrack,
        int ShopId, int PriceHistoryId) : DomainDto(Id);
}
