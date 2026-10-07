using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.LayersParallelBridge.UpsertionInvocation;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.LayersParallelBridge.Scraping
{
    public interface IParallelBridgeCitilinkScraper
    {
        public Task<(string? scrapable, ScrapeState state, int requestsUsed)>
                ScrapeAsync(CitilinkScrapeTarget citilinkScrapeTarget, RequestsQuota quota);
    }
}
