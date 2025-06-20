using PriceTracker.Models;

namespace PriceTracker.Modules.MerchDataProvider.MerchDataExtraction.ExecutionState
{
    public abstract class ExtractionExecutionStateInfo(bool isResumed,
        int id = default) : BaseModel(id)
    {
        public bool IsResumed = isResumed;
    }
}
