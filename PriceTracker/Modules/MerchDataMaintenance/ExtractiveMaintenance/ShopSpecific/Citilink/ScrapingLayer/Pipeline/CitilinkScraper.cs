using PriceTracker.Core.HelperUtils;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Pipeline
{
    public class CitilinkScraper : ICitilinkScraper
    {
        private readonly IInnerCitilinkScraper _innerScraper;
        private readonly ICitilinkScrapeAssignmentFactory _assignmentFactory;
        public CitilinkScraper(IInnerCitilinkScraper innerScraper,
            ICitilinkScrapeAssignmentFactory assignmentFactory)
        {
            _innerScraper = innerScraper;
            _assignmentFactory = assignmentFactory;
        }
        public async Task<FunctionResult<string?, CitilinkExecutionMetadata>>
            ScrapeAsync(CitilinkScrapeTarget nodeLocation, ICitilinkScraperPoliciesEnforcer enforcer)
        {
            CitilinkScrapeAssignment assignment = _assignmentFactory.Create(nodeLocation, enforcer);

            var scrapeResult = await _innerScraper.ScrapeAsync(assignment);

            CitilinkExecutionMetadata pipelineMetadata = new() { ScrapeMetadata = scrapeResult.Info };
            return new(scrapeResult.Result, pipelineMetadata);
        }
    }
}
