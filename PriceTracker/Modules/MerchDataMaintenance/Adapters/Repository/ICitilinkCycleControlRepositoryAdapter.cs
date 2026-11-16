namespace PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository
{
    public interface ICitilinkCycleControlRepositoryAdapter
    {
        public Task<bool> TryGetFullCycleStartAllowanceAsync();
        public Task<int> TryGetPartialCycleRequestsQuotaAsync();
        public Task RegisterSpentRequestsAsync(int spentRequestsCount);
        public Task<(DateTime partialAvailableAt, DateTime fullAvailableAt)>
            GetAllowedCycleStartTime();
    }
}
