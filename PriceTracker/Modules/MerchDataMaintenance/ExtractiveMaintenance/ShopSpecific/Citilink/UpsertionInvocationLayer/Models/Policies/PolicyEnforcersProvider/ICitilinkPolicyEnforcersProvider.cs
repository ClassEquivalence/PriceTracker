namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider
{
    public interface ICitilinkPolicyEnforcersProvider
    {
        public ICitilinkScraperPoliciesEnforcer ProvideForEntireNodeAugmentationEpisode();
    }
}
