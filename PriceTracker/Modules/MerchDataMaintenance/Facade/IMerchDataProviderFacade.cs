namespace PriceTracker.Modules.MerchDataMaintenance.Facade
{
    /// <summary>
    /// Внешний интерфейс модуля.
    /// </summary>
    public interface IMerchDataProviderFacade
    {
        public Task ProcessMerchUpsertion();
        public Task OnShutdownAsync();
    }
}
