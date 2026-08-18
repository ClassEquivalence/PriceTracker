using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Pipeline;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer
{
    public class NewCitilinkOuterParser : ICitilinkParser
    {
        private readonly ICitilinkScraper _scraper;
        private readonly IInnerCitilinkParser _innerParser;

        public NewCitilinkOuterParser(ICitilinkScraper scraper, IInnerCitilinkParser
            parser)
        {
            _scraper = scraper;
            _innerParser = parser;
        }

        public async Task<FunctionResult<CitilinkParsingSnapshot?, CitilinkExecutionMetadata>>
            ParseUnitAsync(CitilinkScrapeTarget nodeLocation, ICitilinkScraperPoliciesEnforcer enforcer)
        {
            var scrapeResult = await _scraper.ScrapeAsync(nodeLocation, enforcer);
            var pipelineMetadata = scrapeResult.Info;
            var previousLayerData = pipelineMetadata.ScrapeMetadata;
            if (previousLayerData.CompletionState == ScrapeCompletionState.Success)
            {
                var scraped = scrapeResult.Result;
                var parseResult = await _innerParser.ParseUnitAsync(scraped);

                var snapshot = parseResult.Result;
                var parseMetadata = parseResult.Info;
                pipelineMetadata.ParseMetadata = parseMetadata;

                return new(snapshot, pipelineMetadata);
            }
            else
            {
                pipelineMetadata.ParseMetadata = new(ParseCompletionState.Failure_PreviousLayer);
                return new(null, pipelineMetadata);
            }
        }
    }
}
