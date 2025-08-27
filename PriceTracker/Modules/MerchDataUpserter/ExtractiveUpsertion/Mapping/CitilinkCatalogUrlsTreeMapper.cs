using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Mapping
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

        private Branch MapBranch(BranchWithHtml branch)
        {
            Branch newInstanceBranch = new(branch.Id, branch.Url,
                branch.Children.Select(MapBranch).ToList(), branch.IsProcessed);
            return newInstanceBranch;
        }

        private BranchWithHtml MapBranch(Branch branch)
        {
            BranchWithHtml newInstanceBranch = new(branch.Id, branch.Url,
                branch.Children.Select(MapBranch).ToList(), branch.IsProcessed);
            return newInstanceBranch;
        }

    }
}
