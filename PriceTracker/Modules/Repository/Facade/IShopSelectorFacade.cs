using PriceTracker.Core.Models.Domain;

namespace PriceTracker.Modules.Repository.Facade
{
    public interface IShopSelectorFacade
    {
        public ShopDto GetCitilinkShop();
    }
}
