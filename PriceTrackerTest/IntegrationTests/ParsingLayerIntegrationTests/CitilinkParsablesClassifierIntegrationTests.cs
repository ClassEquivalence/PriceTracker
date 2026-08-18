using Microsoft.Extensions.Logging;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;
using PriceTrackerTest.Utils.Logging;
using PriceTrackerTest.Utils.TestAppBuilder;
using Xunit.Abstractions;

namespace PriceTrackerTest.IntegrationTests.ParsingLayerIntegrationTests
{
    public class CitilinkParsablesClassifierIntegrationTests
    {
        private readonly ILogger _logger;
        public CitilinkParsablesClassifierIntegrationTests(ITestOutputHelper
            outputHelper)
        {
            _logger = new TestOutputLogger(outputHelper);
        }

        [Theory]
        [InlineData("https://www.citilink.ru/catalog/", ParsableType.RootCatalog)]
        [InlineData("https://www.citilink.ru/catalog/moduli-pamyati/", ParsableType.MerchCatalog)]
        [InlineData("https://www.citilink.ru/catalog/televizory-audio-video-hi-fi/", ParsableType.CatalogsCatalog)]
        public async Task Classify_ClassifiesCorrectly(string url, ParsableType expectedType)
        {
            TestAppBuilder appBuilder = new();
            var app = appBuilder.BuildWithDefaults(_logger);
            ICitilinkParsableClassifier classifier = app.ProvideParsablesClassifier();
            var scraper = app.ProvideScraper();

            CitilinkEnforcersSharedContext context = new();
            CitilinkPolicyEnforcersContextfulProvider citilinkPolicyEnforcersProvider
                = new(context);
            CitilinkScrapeTarget scrapeTarget = new(url);
            var scrapeRes = await scraper.ScrapeAsync(scrapeTarget, citilinkPolicyEnforcersProvider.ProvideForEntireNodeAugmentationEpisode());
            if (scrapeRes.Info.ScrapeMetadata.CompletionState != ScrapeCompletionState.Success
                || scrapeRes.Result == null)
            {
                throw new InvalidOperationException();
            }
            var obtainedType = classifier.Classify(scrapeRes.Result);

            Assert.Equal(expectedType, obtainedType);
        }
    }
}
