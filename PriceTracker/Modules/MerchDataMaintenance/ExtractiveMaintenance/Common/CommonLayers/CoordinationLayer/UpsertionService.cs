using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.LayersParallelBridge.UpsertionInvocation;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.CoordinationLayer
{
    public class UpsertionService
    {
        private readonly CitilinkPartialCycleExecutor _citilinkExecutor;
        public UpsertionService(CitilinkPartialCycleExecutor citilinkExecutor)
        {
            _citilinkExecutor = citilinkExecutor;
        }

        public async Task ProcessUpsertion()
        {
            while (true)
            {
                await _citilinkExecutor.ExecutePartialCycleAsync(200);
            }
        }

        public async Task OnShutdown()
        {
            throw new NotImplementedException();
        }

    }
}
