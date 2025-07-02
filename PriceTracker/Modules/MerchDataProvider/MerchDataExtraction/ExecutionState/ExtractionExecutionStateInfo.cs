

namespace PriceTracker.Modules.MerchDataProvider.MerchDataExtraction.ExecutionState
{
    public abstract class ExtractionExecutionStateInfo(bool isResumed,
        int id = default)
    {
        public int Id = id;
        public bool IsResumed = isResumed;
    }
}
