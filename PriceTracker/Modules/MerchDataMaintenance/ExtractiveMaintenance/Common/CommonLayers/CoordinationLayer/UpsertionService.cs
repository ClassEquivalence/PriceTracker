using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.UpsertionInvocationLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.CoordinationLayer
{
    public class UpsertionService
    {
        private readonly ICitilinkUpsertionInvoker
            _citilinkUpsertionInvoker;
        private readonly List<ScheduledMerchUpserter> _merchUpserters;
        public UpsertionService(ICitilinkUpsertionInvoker citilinkUpsertionInvoker)
        {
            _citilinkUpsertionInvoker = citilinkUpsertionInvoker;
        }

        public async Task ProcessUpsertion()
        {
            while (true)
            {
                CitilinkEnforcersSharedContext sharedContext = new();
                var cycleState = await _citilinkUpsertionInvoker.
                    ExecutePartialCycleAsync(sharedContext, PartialCycleExecutionMode.AutoChoice);
            }
        }

        public async Task OnShutdown()
        {
            Task[] tasks = new Task[_merchUpserters.Count];
            int i = 0;
            foreach (var upserter in _merchUpserters)
            {
                tasks[i] = upserter.OnShutDownAsync();
                i++;
            }
            await Task.WhenAll(tasks);
        }

    }
}
