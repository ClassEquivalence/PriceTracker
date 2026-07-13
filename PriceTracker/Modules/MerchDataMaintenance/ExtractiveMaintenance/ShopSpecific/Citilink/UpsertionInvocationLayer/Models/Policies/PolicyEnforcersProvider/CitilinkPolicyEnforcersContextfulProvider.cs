namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider
{
    public class CitilinkPolicyEnforcersContextfulProvider : ICitilinkPolicyEnforcersContextfulProvider
    {
        private CitilinkEnforcersSharedContext _context;
        public CitilinkPolicyEnforcersContextfulProvider(CitilinkEnforcersSharedContext
            context)
        {
            _context = context;
        }

        public ICitilinkScraperPoliciesEnforcer ProvideForEntireNodeAugmentationEpisode()
        {
            CitilinkScraperPoliciesEnforcer enforcer = new(_context);
            return enforcer;
        }

        public void SetActiveContext(CitilinkEnforcersSharedContext context)
        {
            _context = context;
            // Do something else!!!! (i.e. pass context down to specific enforsers!... no dont do it. nevermind.)
        }
    }
}
