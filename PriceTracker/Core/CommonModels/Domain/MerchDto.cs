
namespace PriceTracker.Core.CommonModels.Domain
{
    public abstract record MerchDto(int Id, string Name, MerchPriceHistoryDto PriceTrack,
        int PriceHistoryId) : DomainDto(Id);
}
