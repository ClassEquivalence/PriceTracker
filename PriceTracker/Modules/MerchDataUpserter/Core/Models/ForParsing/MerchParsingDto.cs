namespace PriceTracker.Modules.MerchDataUpserter.Core.Models.ForParsing
{
    public record MerchParsingDto(string Name, decimal Price) : BaseParsingDto;
}
