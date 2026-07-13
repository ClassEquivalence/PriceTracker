using PriceTracker.Modules.MerchDataMaintenance.Adapters;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Inner.CatalogTreeMaintainer
{
    public class CitilinkCatalogTreeMaintainer : ICitilinkCatalogTreeMaintainer
    {
        ICitilinkCatalogTreeRepositoryAdapter _treeRepository;
        public CitilinkCatalogTreeMaintainer(ICitilinkCatalogTreeRepositoryAdapter treeRepository)
        {
            _treeRepository = treeRepository;
        }

        public async Task FillDescendantsAsync(CitilinkNode parent, List<CitilinkScrapeTarget> descendantsLocationData)
        {
            var nodes = descendantsLocationData.Select(d => new CitilinkNode(d,
                CitilinkNodeAugmentationState.Unprocessed));
            parent.Descendants.AddRange(nodes);
            await _treeRepository.AppendBranchesAsync(parent, descendantsLocationData);
        }

        public async Task PurgeTreeAsync()
        {
            await _treeRepository.RecreateTreeAsync();
        }

        public async Task UpdateNodeAsync(CitilinkNode node)
        {
            await _treeRepository.UpdateNodeAsync(node);
        }
    }
}
