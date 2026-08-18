using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;

namespace PriceTrackerTest.IntegrationTests
{
    public class InnerCitilinkScraperTests
    {
        [Theory]
        [InlineData("noutbuki", 0)]
        [InlineData("noutbuki", 1)]
        public async Task InnerScraper_ScrapesMerchJson(string categorySlug, int page)
        {
            CitilinkScrapeMerchJsonTarget scrapeTarget = new CitilinkScrapeMerchJsonTarget
                (categorySlug, page);

            CitilinkScrapeEpisodeDescriptor descriptor = scrapeTarget.CreateDescriptor();

            var innerScraper = new NewCitilinkScraper(new());

            CitilinkEnforcersSharedContext sharedContext = new();
            CitilinkScraperPoliciesEnforcer enforcer = new(sharedContext);
            CitilinkScrapeAssignment assignment = new(descriptor, enforcer);
            var scrapeRes = await innerScraper.ScrapeAsync(assignment);
            if (scrapeRes.Info.CompletionState != ScrapeCompletionState.Success)
            {
                Assert.Fail($"{scrapeRes.Info.CompletionState} should be " +
                    $"{ScrapeCompletionState.Success}");
            }
        }
    }
}
