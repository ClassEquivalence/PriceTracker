using PriceTracker.Models.DomainModels.ShopSpecificModels.Citilink;

namespace PriceTracker.Modules.MerchDataProvider
{
    /// <summary>
    /// Внешний интерфейс модуля.
    /// </summary>
    public interface IMerchDataProviderFacade
    {
        public Task ProcessMerchUpsertion();
    }
}
