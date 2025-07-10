namespace PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction
{
    public class CitilinkParsingExecutionStateEntity : BaseEntity
    {
        public string CurrentCatalogUrl { get; set; }

        public int CatalogPageNumber { get; set; }

        public bool IsCompleted { get; set; }

        public CitilinkParsingExecutionStateEntity(string currentCatalogUrl,
            int catalogPageNumber, bool isCompleted, int id = default) : base(id)
        {
            CurrentCatalogUrl = currentCatalogUrl;
            CatalogPageNumber = catalogPageNumber;
            IsCompleted = isCompleted;
        }
    }
}
