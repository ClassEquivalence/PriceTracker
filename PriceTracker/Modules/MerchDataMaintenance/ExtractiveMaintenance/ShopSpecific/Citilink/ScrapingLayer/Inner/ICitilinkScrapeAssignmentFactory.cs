using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner
{
    public interface ICitilinkScrapeAssignmentFactory
    {
        CitilinkScrapeAssignment Create(CitilinkScrapeTarget target,
            ICitilinkScraperPoliciesEnforcer policiesEnforcer);
    }
}
