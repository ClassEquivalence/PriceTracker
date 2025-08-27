namespace PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction.CatalogTree
{
    public class CitilinkCatalogBranchEntity: BaseEntity
    {
        public CitilinkCatalogBranchEntity(string currentCatalogUrl,
            bool isBranchProcessed, int id = default) : base(id)
        {
            CurrentCatalogUrl = currentCatalogUrl;
            IsBranchProcessed = isBranchProcessed;
        }

        public CitilinkCatalogBranchEntity(string currentCatalogUrl,
            List<CitilinkCatalogBranchEntity> branches,
            bool isProcessed, int id =default)
            : this(currentCatalogUrl, isProcessed, id)
        {
            Branches = branches;
        }

        public string CurrentCatalogUrl { get; set; }
        public List<CitilinkCatalogBranchEntity> Branches 
        { get; set; }

        public bool IsBranchProcessed { get; set; }

    }
}
