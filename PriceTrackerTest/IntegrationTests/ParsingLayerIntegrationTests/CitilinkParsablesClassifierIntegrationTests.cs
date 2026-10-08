using Microsoft.Extensions.Logging;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;
using PriceTrackerTest.Utils.Logging;
using PriceTrackerTest.Utils.TestAppBuilder;
using Xunit.Abstractions;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;

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
            var app = appBuilder.BuildTestApp(_logger);
            ICitilinkParsableClassifier classifier = app.ProvideParsablesClassifier();
            var scraper = app.ProvideScraper();

            CitilinkScrapeTarget scrapeTarget = new(url);

            RequestsQuota quota = new(200);
            var scrapeRes = await scraper.ScrapeAsync(scrapeTarget, quota);
            if (scrapeRes.state != ScrapeState.Success
                || scrapeRes.scrapable == null)
            {
                throw new InvalidOperationException();
            }
            var obtainedType = classifier.Classify(scrapeRes.scrapable);

            Assert.Equal(expectedType, obtainedType);
        }
    }
}
