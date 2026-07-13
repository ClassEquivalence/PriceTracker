namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.UpsertionLayer
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
