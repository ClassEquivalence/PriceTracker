using PriceTracker.Core.HelperUtils;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner
{
    public interface IInnerCitilinkParser
    {
        public Task<FunctionResult<CitilinkParsingSnapshot, CitilinkParseMetadata>>
            ParseUnitAsync(string parsable);
    }
}
