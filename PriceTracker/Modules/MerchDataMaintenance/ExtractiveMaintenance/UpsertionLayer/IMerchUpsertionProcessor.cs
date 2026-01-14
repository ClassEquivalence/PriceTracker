using PriceTracker.Core.Models.Process;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Models.ForParsing;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.UpsertionLayer
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
