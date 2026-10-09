using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer
{
    public interface ICitilinkScraper
    {
        public Task<(string? scrapable, ScrapeState state, int requestsUsed)>
                ScrapeAsync(CitilinkScrapeTarget citilinkScrapeTarget, RequestsQuota quota);
    }
}
