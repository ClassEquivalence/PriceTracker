namespace PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree
{
    public record Branch: BaseDto
    {
        public int Id;
        public string Url;
        public List<Branch> Children;

        /// <summary>
        /// Ветвь обработана = из неё и её возможных наследников выкачаны все
        /// возможные товары.
        /// </summary>
        public bool IsProcessed;
        public Branch(int id, string url, List<Branch> children, bool isProcessed)
        {
            Id = id;
            Url = url;
            Children = children;
            IsProcessed = isProcessed;
        }
    }
}
