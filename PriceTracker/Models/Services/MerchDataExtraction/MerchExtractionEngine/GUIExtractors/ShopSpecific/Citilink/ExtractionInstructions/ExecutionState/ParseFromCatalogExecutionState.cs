namespace PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink.ExtractionInstructions.ExecutionState
{
    public class ParseFromCatalogExecutionState
    {
        public ParseFromCatalogExecutionState(int pageNumber)
        {
            PageNumber = pageNumber;
        }
        public int PageNumber { get; set; }
    }
}
