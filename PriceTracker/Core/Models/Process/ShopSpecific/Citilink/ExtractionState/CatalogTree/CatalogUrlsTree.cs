using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;

namespace PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree
{
    public record CatalogUrlsTree: BaseDto
    {
        public Branch Root;
        public int Id;
        public CatalogUrlsTree(Branch root, int id = default)
        {
            Id = id;
            Root = root;
        }

        public CatalogUrlsTree DeepClone()
        {
            return new CatalogUrlsTree(Root.DeepClone(), Id);
        }


        public List<Branch> GetAllBranches()
        {
            var all = SelectAllBranchDescendants(Root);
            all.Add(Root);

            return all;
        }

        private List<Branch> SelectAllBranchDescendants(Branch parent)
        {
            List<Branch> branches = [];

            branches.AddRange(parent.Children);

            foreach (Branch branch in parent.Children)
            {
                branches.AddRange(branch.Children);
            }

            return branches;

        }

    }
}
