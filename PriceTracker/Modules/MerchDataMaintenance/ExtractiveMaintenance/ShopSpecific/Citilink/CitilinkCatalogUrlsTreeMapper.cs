using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink
{
    public class CitilinkCatalogUrlsTreeMapper
    {
        public CitilinkCatalogUrlsTree Map(CatalogUrlsTree tree)
        {
            CitilinkCatalogUrlsTree newInstanceTree = new(MapBranch(tree.Root), tree.Id);
            return newInstanceTree;
        }

        public CatalogUrlsTree Map(CitilinkCatalogUrlsTree tree)
        {
            CatalogUrlsTree newInstanceTree = new(MapBranch(tree.Root), tree.Id);
            return newInstanceTree;
        }

        private Branch MapBranch(BranchWithFunctionality branch)
        {
            Branch newInstanceBranch = new(branch.Id, branch.Url,
                branch.Children.Select(MapBranch).ToList(), branch.IsProcessed);
            return newInstanceBranch;
        }

        private BranchWithFunctionality MapBranch(Branch branch)
        {
            BranchWithFunctionality newInstanceBranch = new(branch.Id, branch.Url,
                branch.Children.Select(MapBranch).ToList(), branch.IsProcessed);
            return newInstanceBranch;
        }

    }
}
