using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Pipeline;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models
{
    public class CitilinkExecutionMetadata
    {
        public CitilinkScrapeMetadata? ScrapeMetadata;
        public CitilinkParseMetadata? ParseMetadata;
        public CitilinkNodeAugmentMetadata? DataSyncMetadata;
    }
}
