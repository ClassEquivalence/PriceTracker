namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies
{
    public interface ICitilinkScraperPoliciesEnforcer
    {

        bool TryRegisterScrapeRequest();
    }

}
