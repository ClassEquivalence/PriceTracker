using PriceTracker.Models.Services.MerchDataExtraction.ExecutionState;

namespace PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink
{
    public class CitilinkParsingExecutionState : ExtractionExecutionStateInfo
    {
        public string CurrentCatalogUrl { get; set; }
        public int CatalogPageNumber { get; set; }

        public CitilinkParsingExecutionState(string currentCatalogUrl,
            int pageNumber, bool isResumed, int id=default):
            base(isResumed, id)
        {
            CurrentCatalogUrl = currentCatalogUrl;
            CatalogPageNumber = pageNumber;
        }
    }
}
