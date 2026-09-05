using PriceTracker.Core.HelperUtils;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner
{
    public interface IInnerCitilinkScraper
    {
        public Task<FunctionResult<string?, CitilinkScrapeMetadata>>
            ScrapeAsync(CitilinkScrapeAssignment assignment);
    }
}
