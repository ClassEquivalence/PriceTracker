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
    }
}
