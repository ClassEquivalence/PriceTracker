
namespace PriceTracker.Core.Models.Domain
{
    public record MerchPriceHistoryDto(int Id, List<TimestampedPriceDto>
        PreviousTimestampedPricesList, TimestampedPriceDto CurrentPrice,
        int MerchId, int CurrentPricePointerId = default) : DomainDto(Id)
    {

    }
}
