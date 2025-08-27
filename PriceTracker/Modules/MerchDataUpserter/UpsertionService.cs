using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion
{
    public class UpsertionService
    {
        private readonly List<ScheduledMerchUpserter> _merchUpserters;
        public UpsertionService(List<ScheduledMerchUpserter>
            scheduledUpserters)
        {
            _merchUpserters = scheduledUpserters;
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
