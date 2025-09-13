using System.Security.Cryptography.X509Certificates;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink
{
    public record CitilinkCatalogUrlsTree
    {
        public int Id;
        public BranchWithFunctionality Root;

        public event Action? FiltersAndDuplicatesRemoved;

        public CitilinkCatalogUrlsTree(BranchWithFunctionality root, int id=default)
        {
            Id = id;
            Root = root;
        }

        public void RemoveBranchFiltersAndDuplicates()
        {
            List<BranchWithFunctionality> all = GetAllBranches();

            foreach(var branch in all)
            {
                branch.RemoveFilter();
            }

            RecursiveRemoveDuplicateBranches(Root);

            FiltersAndDuplicatesRemoved?.Invoke();

            void RecursiveRemoveDuplicateBranches(BranchWithFunctionality parent)
            {

                var children = new List<BranchWithFunctionality>(parent.Children);

                foreach (BranchWithFunctionality branch in children)
                {
                    if (all.Count(b => b.Url == branch.Url) > 1)
                    {
                        var isRemoved = parent.Children.Remove(branch);
                        var isRemovedFromAll = all.Remove(branch);
                        if (!(isRemoved && isRemovedFromAll))
                            throw new InvalidOperationException($"{nameof(CitilinkCatalogUrlsTree)}, " +
                                $"{nameof(RecursiveRemoveDuplicateBranches)}: Во время очистки древа от дублей произошел сбой.");
                        continue;
                    }

                    RecursiveRemoveDuplicateBranches(branch);
                }
            }

        }

        

        public int BranchWithUrlCount(string url)
        {
            return (GetAllBranches().Count(b => b.Url == url));
        }

        public List<BranchWithFunctionality> GetAllBranches()
        {
            var all = SelectAllBranchDescendants(Root);
            all.Add(Root);

            return all;
        }

        private List<BranchWithFunctionality> SelectAllBranchDescendants(BranchWithFunctionality parent)
        {
            List<BranchWithFunctionality> branches = [];

            branches.AddRange(parent.Children);

            foreach(BranchWithFunctionality branch in parent.Children)
            {
                branches.AddRange(branch.Children);
            }

            return branches;

        }

    }
}
