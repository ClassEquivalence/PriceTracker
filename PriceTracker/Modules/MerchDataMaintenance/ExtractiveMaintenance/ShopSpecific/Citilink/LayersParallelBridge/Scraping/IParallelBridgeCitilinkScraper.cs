using PriceTracker.Core.HelperUtils;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.LayersParallelBridge.UpsertionInvocation;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.LayersParallelBridge.Scraping
{
    public interface IParallelBridgeCitilinkScraper
    {
        public Task<(string? scrapable, ScrapeState state, int requestsUsed)>
                ScrapeAsync(CitilinkScrapeTarget citilinkScrapeTarget, RequestsQuota quota);
    }
}
