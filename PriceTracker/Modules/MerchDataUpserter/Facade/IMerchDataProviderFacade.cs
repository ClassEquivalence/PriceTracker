namespace PriceTracker.Modules.MerchDataUpserter.Facade
{
    /// <summary>
    /// Внешний интерфейс модуля.
    /// </summary>
    public interface IMerchDataProviderFacade : IHostedService
    {
        public Task ProcessMerchUpsertion();
        public Task OnShutdownAsync();
    }
}
