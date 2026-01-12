using PriceTracker.Core.Models.Process;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ForParsing;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services
{
    public interface IMerchUpsertionProcessor
    {

        enum UpsertionSubcycleResult
        {
            SubcycleCompleted,
            EntireCycleCompleted
        }

        public Task<UpsertionSubcycleResult> ProcessUpsertionSubCycleAsync();
        public Task OnProcessHalt();
    }
}
