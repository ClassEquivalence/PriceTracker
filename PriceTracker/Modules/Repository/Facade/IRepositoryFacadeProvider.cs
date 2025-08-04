using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;

namespace PriceTracker.Modules.Repository.Facade
{
    public interface IRepositoryFacadeProvider
    {
        ICitilinkMerchRepositoryFacade GetCitilinkMerchRepository();
        ICitilinkMiscellaneousRepositoryFacade GetCitilinkMiscellaneousRepository();
        IPriceHistoryRepositoryFacade GetPriceHistoryRepository();
        IShopRepositoryFacade GetShopRepository();
        IShopSelectorFacade GetShopSelector();
        ITimestampedPriceRepositoryFacade GetTimestampedPriceRepository();
        ICitilinkMerchRepositoryFacade GetBufferedCitilinkMerchRepository();

    }
}
