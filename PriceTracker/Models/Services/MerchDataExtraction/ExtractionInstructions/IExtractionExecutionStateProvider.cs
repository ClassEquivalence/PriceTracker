namespace PriceTracker.Models.Services.MerchDataExtraction.ExtractionInstructions
{
    public interface IExtractionExecutionStateProvider
        <ExtractionStateInfo> where ExtractionStateInfo: ExtractionAgentExecutionStateInfo
    {
        public ExtractionStateInfo Provide();
    }
}
