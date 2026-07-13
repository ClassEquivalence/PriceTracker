namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider
{
    public interface ICitilinkPolicyEnforcersContextfulProvider : ICitilinkPolicyEnforcersProvider
    {
        void SetActiveContext(CitilinkEnforcersSharedContext context);
    }
}
