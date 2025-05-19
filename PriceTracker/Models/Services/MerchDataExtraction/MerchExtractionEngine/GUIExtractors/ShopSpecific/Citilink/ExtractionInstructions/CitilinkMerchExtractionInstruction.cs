using PriceTracker.Models.Services.MerchDataExtraction.ExtractionInstructions;
using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink.ExtractionInstructions.ExecutionState;

namespace PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink.ExtractionInstructions
{
    public class CitilinkMerchExtractionInstruction: ExtractionAgentExecutionStateInfo
    {
        public ParsingExecutionState ExecutionState;
        public CitilinkMerchExtractionInstruction(
            ParsingExecutionState executionState, bool continueFromExecutionState)
        {
            ExecutionState = executionState;
            ExtractionProcessNew = !continueFromExecutionState;
        }
    }
}
