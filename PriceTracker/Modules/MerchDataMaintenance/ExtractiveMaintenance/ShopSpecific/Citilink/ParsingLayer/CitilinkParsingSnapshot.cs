using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;

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
