using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Inner.CatalogTreeMaintainer
{
    public interface ICitilinkCatalogTreeMaintainer
    {
        Task PurgeTreeAsync();
        Task FillDescendantsAsync(CitilinkNode parent, List<CitilinkScrapeTarget>
            descendantsLocationData);

        Task UpdateNodeAsync(CitilinkNode node);
    }
}
