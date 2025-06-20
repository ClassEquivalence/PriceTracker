using PriceTracker.Modules.MerchDataProvider.MerchDataExtraction.ExecutionState;

namespace PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine.ShopSpecific.Citilink
{
    public class CitilinkParsingExecutionState : ExtractionExecutionStateInfo
    {
        public string CurrentCatalogUrl { get; set; }
        public int CatalogPageNumber { get; set; }

        public CitilinkParsingExecutionState(string currentCatalogUrl,
            int pageNumber, bool isResumed, int id = default) :
            base(isResumed, id)
        {
            CurrentCatalogUrl = currentCatalogUrl;
            CatalogPageNumber = pageNumber;
        }
    }
}
