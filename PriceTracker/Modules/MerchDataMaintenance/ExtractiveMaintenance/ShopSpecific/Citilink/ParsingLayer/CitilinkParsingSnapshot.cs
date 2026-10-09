using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer
{
    public class CitilinkParsingSnapshot
    {
        public ParsableType? Type;
        public List<string>? DescendantNodeUrls;
        public List<CitilinkMerchParsingDto>? Merches;
        public int? NodeMerchesCount;
    }
}
