using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;

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
            var innerScraper = new CitilinkScraper(new());
            RequestsQuota quota = new(5);
            var scrapeRes = await innerScraper.ScrapeAsync(scrapeTarget, quota);
            if (scrapeRes.state != ScrapeState.Success)
            {
                Assert.Fail($"{scrapeRes.state} should be " +
                    $"{ScrapeState.Success}");
            }
        }
    }
}
