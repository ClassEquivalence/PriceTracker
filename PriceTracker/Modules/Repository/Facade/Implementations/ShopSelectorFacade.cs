using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;

namespace PriceTracker.Modules.Repository.Facade.Implementations
{
    public class ShopSelectorFacade : IShopSelectorFacade
    {

        private readonly ShopRepository _shopRepository;
        public ShopSelectorFacade(ShopRepository shopRepository)
        {
            _shopRepository = shopRepository;
        }
        public ShopDto GetCitilinkShop()
        {
            return _shopRepository.SingleOrDefault(shop => shop.Name == RepositoryParameters.CitilinkShopName)
                ?? throw new InvalidOperationException("Магазин Citilink не инициализирован.");
        }
    }
}
