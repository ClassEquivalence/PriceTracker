using System.Security.Cryptography.X509Certificates;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink
{
    public record CitilinkCatalogUrlsTree
    {
        public int Id;
        public BranchWithHtml Root;
        public CitilinkCatalogUrlsTree(BranchWithHtml root, int id=default)
        {
            Id = id;
            Root = root;
        }

        public void RemoveBranchFiltersAndDuplicates()
        {
            List<BranchWithHtml> all = GetAllBranches();

            RecursiveRemoveDuplicateBranchesAndFilters(Root);

            
            void RecursiveRemoveDuplicateBranchesAndFilters(BranchWithHtml parent)
            {
                parent.RemoveFilter();

                var children = new List<BranchWithHtml>(parent.Children);

                foreach (BranchWithHtml branch in children)
                {
                    if (all.Count(b => b.Url == branch.Url) > 1)
                    {
                        parent.Children.Remove(branch);
                        all.Remove(branch);
                        continue;
                    }

                    RecursiveRemoveDuplicateBranchesAndFilters(branch);
                }
            }


            
        }

        

        public int BranchWithUrlCount(string url)
        {
            return (GetAllBranches().Count(b => b.Url == url));
        }

        public List<BranchWithHtml> GetAllBranches()
        {
            var all = SelectAllBranchDescendants(Root);
            all.Add(Root);

            return all;
        }

        private List<BranchWithHtml> SelectAllBranchDescendants(BranchWithHtml parent)
        {
            List<BranchWithHtml> branches = [];

            branches.AddRange(parent.Children);

            foreach(BranchWithHtml branch in parent.Children)
            {
                branches.AddRange(branch.Children);
            }

            return branches;

        }

    }
}
