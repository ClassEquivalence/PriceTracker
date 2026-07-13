using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.ParsingLayer.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models
{
    public record CitilinkMerchParsingDto(decimal Price, string CitilinkId, string Name) : MerchParsingDto(Name, Price)
    {
        public override string ToString()
        {
            return $"Name: {Name}, Price: {Price}, CitilinkId: {CitilinkId}";
        }
    }
}
