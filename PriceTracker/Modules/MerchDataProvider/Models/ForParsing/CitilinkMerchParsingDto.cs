namespace PriceTracker.Modules.MerchDataProvider.Models.ForParsing
{
    public record CitilinkMerchParsingDto(decimal Price, string CitilinkId, string Name) : MerchParsingDto(Name, Price);
}
