using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner.Parsers;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Pipeline;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;
using PriceTrackerTest.Utils.Logging;
using Xunit.Abstractions;

namespace PriceTrackerTest.IntegrationTests.ParsingLayerIntegrationTests
{
    public class OuterCitilinkParserIntegrationTests
    {
        private readonly NewCitilinkOuterParser _outerParser;
        private readonly ITestOutputHelper _testOutput;
        public OuterCitilinkParserIntegrationTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;

            NewCitilinkScraper innerScraper = new(new(), new TestOutputLogger(testOutput));
            CitilinkScrapeAssignmentFactory assignmentFactory = new();
            CitilinkScraper scraper = new CitilinkScraper(innerScraper, 
                assignmentFactory);


            CitilinkUpsertionOptions options = new()
            {
                CitilinkUrl = "https://www.citilink.ru/"
            };

            IOptions<CitilinkUpsertionOptions> interfacedOptions =
                Options.Create(options);

            CitilinkParsablesClassifier parsablesClassifier = new();
            CitilinkCatalogsCatalogParser catalogsCatalogParser = new(interfacedOptions);
            CitilinkMerchCatalogParser merchCatalogParser = new();
            CitilinkRootCatalogParser rootCatalogParser = new(interfacedOptions);

            InnerCitilinkParser innerParser = new(parsablesClassifier, catalogsCatalogParser,
                merchCatalogParser, rootCatalogParser);

            _outerParser = new(scraper, innerParser);
        }

        [Theory]
        [InlineData("https://www.citilink.ru/catalog/")]
        [InlineData("https://www.citilink.ru/catalog/processory/?ref=mainmenu")]
        [InlineData("https://www.citilink.ru/catalog/komplektuyuschie-dlya-pk/?ref=mainmenu_crumbs")]
        public async Task Parser_ParsesPages(string url)
        {
            CitilinkScrapeTarget scrapeTarget = new(url, CitilinkScrapeTargetPurpose.Default);
            scrapeTarget.CreateDescriptor();
            CitilinkEnforcersSharedContext sharedContext = new();
            CitilinkPolicyEnforcersContextfulProvider provider = new(sharedContext);
            var parseRes = (await _outerParser.ParseUnitAsync(scrapeTarget, provider.ProvideForEntireNodeAugmentationEpisode()))
                .Result;
            
            if(parseRes.Merches != null)
            {
                _testOutput.WriteLine("Merches section starts:");
                foreach (var merch in parseRes.Merches)
                {
                    _testOutput.WriteLine($"Name: {merch.Name}, " +
                        $"CitilinkID: {merch.CitilinkId}, Price: {merch.Price}");
                }
                _testOutput.WriteLine("Merches section ends.");
            }
            if(parseRes.DescendantNodeUrls != null)
            {
                _testOutput.WriteLine("Descendant urls section starts:");
                foreach (var descendant in parseRes.DescendantNodeUrls)
                {
                    _testOutput.WriteLine($"Url: {descendant}");
                }
                _testOutput.WriteLine("Descendant urls section ends.");
            }
            
            _testOutput.WriteLine($"GuessedType: {parseRes.Type}");
        }

        [Theory]
        [InlineData("noutbuki", 0)]
        [InlineData("noutbuki", 1)]
        public async Task Parser_ParsesMerchJson(string categorySlug, int page)
        {
            CitilinkScrapeMerchJsonTarget scrapeTarget = new CitilinkScrapeMerchJsonTarget
                (categorySlug, page);
            CitilinkEnforcersSharedContext sharedContext = new();
            CitilinkPolicyEnforcersContextfulProvider provider = new(sharedContext);

            var layerRes = await _outerParser.ParseUnitAsync(scrapeTarget, provider.ProvideForEntireNodeAugmentationEpisode());

            var parseRes = layerRes.Result;

            var infoRes = layerRes.Info;

            _testOutput.WriteLine(scrapeTarget.CreateDescriptor().ToString());

            _testOutput.WriteLine($"{infoRes.ParseMetadata.State}, " +
                $"{infoRes.ScrapeMetadata.CompletionState}");

            if (parseRes.Merches != null)
            {
                _testOutput.WriteLine("Merches section starts:");
                foreach (var merch in parseRes.Merches)
                {
                    _testOutput.WriteLine($"Name: {merch.Name}, " +
                        $"CitilinkID: {merch.CitilinkId}, Price: {merch.Price}");
                }
                _testOutput.WriteLine("Merches section ends.");
            }
            if (parseRes.DescendantNodeUrls != null)
            {
                _testOutput.WriteLine("Descendant urls section starts:");
                foreach (var descendant in parseRes.DescendantNodeUrls)
                {
                    _testOutput.WriteLine($"Url: {descendant}");
                }
                _testOutput.WriteLine("Descendant urls section ends.");
            }

            _testOutput.WriteLine($"GuessedType: {parseRes.Type}");
        }
    }
}
