
namespace PriceTracker.Core.CommonModels.Domain
{
    public record MerchPriceHistoryDto(int Id, List<TimestampedPriceDto>
        PreviousTimestampedPricesList, TimestampedPriceDto CurrentPrice,
        int MerchId, int CurrentPricePointerId = default) : DomainDto(Id)
    {

    }
}
