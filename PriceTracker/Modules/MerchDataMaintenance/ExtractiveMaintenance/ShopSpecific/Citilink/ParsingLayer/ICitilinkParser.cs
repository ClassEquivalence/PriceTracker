using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer
{
    public interface ICitilinkParser
    {
        public Task<FunctionResult<CitilinkParsingSnapshot?, CitilinkExecutionMetadata>>
            ParseUnitAsync(CitilinkScrapeTarget nodeLocation, ICitilinkScraperPoliciesEnforcer enforcer);
    }
}
