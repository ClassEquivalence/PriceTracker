using PriceTracker.Models.Services.MerchDataExtraction.ExecutionState;

namespace PriceTracker.Modules.Repository.DataAccess.Repositories.Process
{
    public interface IExtractionExecutionStateProvider
        <ExtractionStateInfo> where ExtractionStateInfo : ExtractionExecutionStateInfo
    {
        public ExtractionStateInfo Provide();
    }
}
