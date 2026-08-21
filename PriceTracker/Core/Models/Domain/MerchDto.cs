
namespace PriceTracker.Core.Models.Domain
{
    public abstract record MerchDto(int Id, string Name, MerchPriceHistoryDto PriceTrack,
        int PriceHistoryId) : DomainDto(Id);
}
