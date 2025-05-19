namespace PriceTracker.Models.DTOModels.ForParsing
{
    public record CitilinkMerchParsingDto(decimal Price, string CitilinkId, string Name): MerchParsingDto(Name, Price);
}
