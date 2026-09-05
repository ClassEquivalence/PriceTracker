namespace PriceTracker.Core.CommonModels.Domain
{
    public record TimestampedPriceDto(int Id, decimal Price, DateTime DateTime,
        int MerchPriceHistoryId) : DomainDto(Id);
}
