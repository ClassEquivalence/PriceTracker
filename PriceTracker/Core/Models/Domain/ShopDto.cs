
namespace PriceTracker.Core.Models.Domain
{
    public record ShopDto(int Id, string Name, List<MerchDto> Merches) : DomainDto(Id);
}
