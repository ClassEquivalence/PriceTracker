
namespace PriceTracker.Modules.MerchDataProvider
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
