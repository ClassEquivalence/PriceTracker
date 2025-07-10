namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink
{
    public class CitilinkParsingExecutionState : ExtractionState
    {
        public string CurrentCatalogUrl { get; set; }
        public int CatalogPageNumber { get; set; }

        public CitilinkParsingExecutionState(string currentCatalogUrl,
            int pageNumber, bool isCompleted, int id = default) :
            base(isCompleted, id)
        {
            CurrentCatalogUrl = currentCatalogUrl;
            CatalogPageNumber = pageNumber;
        }
    }
}
