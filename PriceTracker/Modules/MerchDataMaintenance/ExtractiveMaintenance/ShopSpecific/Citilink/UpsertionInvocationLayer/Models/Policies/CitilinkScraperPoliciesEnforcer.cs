using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies
{
    public class CitilinkScraperPoliciesEnforcer : ICitilinkScraperPoliciesEnforcer
    {
        private readonly CitilinkEnforcersSharedContext _sharedContext;

        // Remaining augmentation episode requests quota
        private int remainingEpisodeQuota;
        public event Action? RequestRegistered;

        public CitilinkScraperPoliciesEnforcer(CitilinkEnforcersSharedContext context)
        {
            _sharedContext = context;
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(context.MaxRequestsPerAugmentationEpisode);
            remainingEpisodeQuota = context.MaxRequestsPerAugmentationEpisode;
        }

        private void RegisterScrapeRequest()
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(remainingEpisodeQuota);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_sharedContext.
                RequestsPerPartialCycleRemainingQuota);
            remainingEpisodeQuota--;
            _sharedContext.RequestsPerPartialCycleRemainingQuota--;
            RequestRegistered?.Invoke();
        }
        private bool CanScrapeRequest()
        {
            bool partialCycleQuotaExceeded = _sharedContext.RequestsPerPartialCycleRemainingQuota <= 0;
            bool scrapeEpisodeQuotaExceeded = remainingEpisodeQuota <= 0;
            return !partialCycleQuotaExceeded && !scrapeEpisodeQuotaExceeded;
        }

        public bool TryRegisterScrapeRequest()
        {
            lock (this)
            {
                if (CanScrapeRequest())
                {
                    RegisterScrapeRequest();
                    return true;
                }
                else
                    return false;
            }
        }
    }
}
