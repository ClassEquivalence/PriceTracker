using PriceTracker.Core.Models.Domain;

namespace PriceTracker.Modules.Repository.Facade.FacadeInterfaces
{
    public interface IShopSelectorFacade
    {
        public ShopDto GetCitilinkShop();
    }
}
