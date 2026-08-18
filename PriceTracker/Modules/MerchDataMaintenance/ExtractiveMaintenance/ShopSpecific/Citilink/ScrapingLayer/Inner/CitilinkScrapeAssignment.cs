using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;
using System.Net;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner
{
    public class CitilinkScrapeAssignment
    {

        public CitilinkScrapeAssignment(string uriString, HttpMethod requestMethod,
            List<HttpStatusCode> allowedStatusCodes, CitilinkEnforcersSharedContext
            sharedContext, Dictionary<string, string>? cookies = null) : this(new(uriString, requestMethod,
                allowedStatusCodes, cookies), new CitilinkScraperPoliciesEnforcer(sharedContext))
        {
        }

        public CitilinkScrapeAssignment(CitilinkScrapeEpisodeDescriptor descriptor,
            ICitilinkScraperPoliciesEnforcer enforcer)
        {
            ScrapeEpisodeDescriptor = descriptor;
            PoliciesEnforcer = enforcer;
        }
        public CitilinkScrapeEpisodeDescriptor ScrapeEpisodeDescriptor;
        public ICitilinkScraperPoliciesEnforcer PoliciesEnforcer;
    }
}
