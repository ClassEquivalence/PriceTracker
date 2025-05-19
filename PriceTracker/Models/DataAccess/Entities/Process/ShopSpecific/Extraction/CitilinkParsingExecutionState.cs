using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink.ExtractionInstructions.ExecutionState;

namespace PriceTracker.Models.DataAccess.Entities.Process.ShopSpecific.Extraction
{
    public class CitilinkParsingExecutionState
    {
        public string CurrentCatalogUrl { get; set; }

        public int CatalogPageNumber { get; set; }

        public CitilinkParsingExecutionState(string currentCatalogUrl,
            int catalogPageNumber)
        {
            CurrentCatalogUrl = currentCatalogUrl;
            CatalogPageNumber = catalogPageNumber;
        }
    }
}
