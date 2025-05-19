namespace PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink.ExtractionInstructions.ExecutionState
{
    public class ParsingExecutionState
    {
        public string CurrentCatalogUrl { get; set; }
        public ParseFromCatalogExecutionState CatalogRetreiveExecState { get; init; }

        public ParsingExecutionState(string currentCatalogUrl,
            ParseFromCatalogExecutionState catalogRetreiveExecState)
        {
            CurrentCatalogUrl = currentCatalogUrl;
            CatalogRetreiveExecState = catalogRetreiveExecState;
        }
    }
}
