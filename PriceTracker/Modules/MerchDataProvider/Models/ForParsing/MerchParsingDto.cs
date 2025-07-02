namespace PriceTracker.Modules.MerchDataProvider.Models.ForParsing
{
    public record MerchParsingDto(string Name, decimal Price) : BaseParsingDto;
}
