using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ForParsing;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink
{
    public record CitilinkMerchParsingDto(decimal Price, string CitilinkId, string Name) : MerchParsingDto(Name, Price)
    {
        public override string ToString()
        {
            return $"Name: {Name}, Price: {Price}, CitilinkId: {CitilinkId}";
        }
    }
}
