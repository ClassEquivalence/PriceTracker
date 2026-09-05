namespace PriceTracker.Core.CommonModels.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree
{
    public record CatalogUrlsTree : BaseDto
    {
        public CitilinkBranch Root;
        public int Id;
        public CatalogUrlsTree(CitilinkBranch root, int id = default)
        {
            Id = id;
            Root = root;
        }

        public CatalogUrlsTree DeepClone()
        {
            return new CatalogUrlsTree(Root.DeepClone(), Id);
        }

        public List<CitilinkBranch> GetAllBranches()
        {
            var all = SelectAllBranchDescendants(Root);
            all.Add(Root);
            return all;
        }


        private List<CitilinkBranch> SelectAllBranchDescendants(CitilinkBranch parent)
        {
            List<CitilinkBranch> branches = [];
            branches.AddRange(parent.Children);
            foreach (CitilinkBranch branch in parent.Children)
            {
                branches.AddRange(SelectAllBranchDescendants(branch));
            }
            return branches;
        }
        public override string ToString()
        {
            return $"CatalogUrlsTree: Id is {Id}, Root is {Root}";
        }
    }
}
