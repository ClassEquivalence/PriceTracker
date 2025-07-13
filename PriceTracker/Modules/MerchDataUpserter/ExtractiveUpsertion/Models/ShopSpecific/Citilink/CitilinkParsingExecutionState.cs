using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink
{
    public class CitilinkParsingExecutionState : ExtractionState
    {
        public string CurrentCatalogUrl { get; set; }
        public int CatalogPageNumber { get; set; }

        public CitilinkParsingExecutionState(string currentCatalogUrl,
            int pageNumber, bool isCompleted) :
            base(isCompleted)
        {
            CurrentCatalogUrl = currentCatalogUrl;
            CatalogPageNumber = pageNumber;
        }
    }
}
