namespace PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction.CatalogTree
{
    public class CitilinkCatalogUrlsTreeEntity: BaseEntity
    {
        public CitilinkCatalogUrlsTreeEntity(int id = default) : base(id)
        {
        }

        public CitilinkCatalogUrlsTreeEntity(CitilinkCatalogBranchEntity root, 
            int id = default) : base(id)
        {
            Root = root;
        }

        public CitilinkCatalogBranchEntity Root { get; set; }
    }
}
