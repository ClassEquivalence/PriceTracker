using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.UpsertionInvocationLayer;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.CoordinationLayer
{
    public class UpsertionService
    {
        private readonly List<ScheduledMerchUpserter> _merchUpserters;
        public UpsertionService(IEnumerable<ScheduledMerchUpserter>
            scheduledUpserters)
        {
            _merchUpserters = scheduledUpserters.ToList();
        }

        public async Task ProcessUpsertion()
        {
            Task[] tasks = new Task[_merchUpserters.Count];
            int i = 0;
            foreach (var upserter in _merchUpserters)
            {
                tasks[i] = upserter.ProcessUpsertion();
                i++;
            }
            await Task.WhenAll(tasks);
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
