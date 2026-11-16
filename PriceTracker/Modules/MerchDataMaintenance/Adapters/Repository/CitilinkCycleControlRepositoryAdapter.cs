using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Modules.Repository.DataAccess.EFCore;

namespace PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository
{
    public class CitilinkCycleControlRepositoryAdapter :
        ICitilinkCycleControlRepositoryAdapter
    {
        private readonly IDbContextFactory<PriceTrackerContext> _contextFactory;
        private readonly CitilinkUpsertionOptions _options;
        public CitilinkCycleControlRepositoryAdapter(IDbContextFactory<PriceTrackerContext> 
            contextFactory, IOptions<CitilinkUpsertionOptions> options)
        {
            _contextFactory = contextFactory;
            _options = options.Value;
        }

        public async Task<(DateTime partialAvailableAt, DateTime fullAvailableAt)> 
            GetAllowedCycleStartTime()
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var execState = await context.CitilinkParsingExecutionStateEntity.SingleAsync();

            var whenPartialAvailable = execState.LastTimePartialStarted +
                TimeSpan.FromHours(_options.MaxPageRequestsPerTime);
            var whenFullAvailable = execState.LastTimeFullStarted +
                TimeSpan.FromDays(_options.CitilinkPriceUpdatePeriod);
            return (whenPartialAvailable ?? DateTime.UtcNow, 
                whenFullAvailable ?? DateTime.UtcNow);
        }
        public async Task RegisterSpentRequestsAsync(int spentRequestsCount)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var execState = await context.CitilinkParsingExecutionStateEntity.SingleAsync();
            execState.PartialCycleRemainingRequestsQuota -= spentRequestsCount;
            if (execState.PartialCycleRemainingRequestsQuota < 0)
                execState.PartialCycleRemainingRequestsQuota = 0;
            await context.SaveChangesAsync();
        }
        public async Task<bool> TryGetFullCycleStartAllowanceAsync()
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var executionState = await dbContext.CitilinkParsingExecutionStateEntity
                .SingleAsync();
            if(executionState.LastTimeFullStarted + TimeSpan.FromHours(_options.CitilinkPriceUpdatePeriod)
                < DateTime.UtcNow
                &&
                executionState.LastTimePartialStarted + TimeSpan.FromHours(_options.MaxPageRequestsPerTime)
                < DateTime.UtcNow)
            {
                executionState.LastTimeFullStarted = DateTime.UtcNow;
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<int> TryGetPartialCycleRequestsQuotaAsync()
        {
            using var dbContext = await _contextFactory.CreateDbContextAsync();
            var executionState = await dbContext.CitilinkParsingExecutionStateEntity
                .SingleAsync();
            if (executionState.LastTimePartialStarted + TimeSpan.FromHours(_options.MaxPageRequestsPerTime)
                < DateTime.UtcNow)
            {
                executionState.LastTimePartialStarted = DateTime.UtcNow;
                executionState.PartialCycleRemainingRequestsQuota = 
                    _options.MaxPageRequestsPerTime;
                await dbContext.SaveChangesAsync();
                return _options.MaxPageRequestsPerTime;
            }
            else
            {
                return executionState.PartialCycleRemainingRequestsQuota;
            }
        }
    }
}
