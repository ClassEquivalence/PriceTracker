using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Models.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.CatalogBranchParsing;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink
{
    public class MerchCatalogUrlsParserFactory
    {
        private readonly ICitilinkScraper _scraper;
        private readonly CitilinkUpsertionOptions _options;
        private readonly ILogger? _logger;
        public MerchCatalogUrlsParserFactory(ICitilinkScraper scraper, 
            CitilinkUpsertionOptions options, ILogger? logger = null)
        {
            _scraper = scraper;
            _options = options;
            _logger = logger;
        }

        public CitilinkMerchCatalogUrlsParser Create(CatalogUrlsTreeProvider catalogUrls)
        {
            return new(_scraper, catalogUrls, _options, _logger);
        }

    }
}
