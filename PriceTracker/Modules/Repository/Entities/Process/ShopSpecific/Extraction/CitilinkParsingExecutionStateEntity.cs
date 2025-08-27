using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction.CatalogTree;

namespace PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction
{
    public class CitilinkParsingExecutionStateEntity : BaseEntity
    {
        public CitilinkCatalogUrlsTreeEntity? CatalogUrls {  get; set; }

        public bool IsCompleted { get; set; }

        public CitilinkParsingExecutionStateEntity(bool isCompleted, 
            int id = default) : base(id)
        {
            IsCompleted = isCompleted;
        }

        public CitilinkParsingExecutionStateEntity(bool isCompleted, 
            CitilinkCatalogUrlsTreeEntity? catalogUrlsTree, 
            int id = default) : base(id)
        {
            IsCompleted = isCompleted;
            CatalogUrls = catalogUrlsTree;
        }

    }
}
