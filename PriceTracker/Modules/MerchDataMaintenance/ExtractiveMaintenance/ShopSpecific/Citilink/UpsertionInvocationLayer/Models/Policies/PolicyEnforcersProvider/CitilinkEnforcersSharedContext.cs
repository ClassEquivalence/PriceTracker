namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider
{
    public class CitilinkEnforcersSharedContext
    {
        // What's request in this context?
        public int MaxRequestsPerAugmentationEpisode;
        public int MaxRequestsPerPartialCycle;

        public int RequestsPerPartialCycleRemainingQuota;

        public CitilinkEnforcersSharedContext(int maxRequestsPerAugmentationEpisode = 5,
            int maxRequestsPerPartialCycle = 200)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxRequestsPerPartialCycle);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxRequestsPerAugmentationEpisode);
            MaxRequestsPerAugmentationEpisode = maxRequestsPerAugmentationEpisode;
            MaxRequestsPerPartialCycle = maxRequestsPerPartialCycle;
            RequestsPerPartialCycleRemainingQuota = MaxRequestsPerPartialCycle;
        }

    }
}
