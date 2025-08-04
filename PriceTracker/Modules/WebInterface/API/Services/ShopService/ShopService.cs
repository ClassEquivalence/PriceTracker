using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.WebInterface.API.DTOModels.Shop;
using PriceTracker.Modules.WebInterface.API.Mapping.MapperProvider;
using PriceTracker.Modules.WebInterface.API.Mapping.Shop;

namespace PriceTracker.Modules.WebInterface.API.Services.ShopService
{
    public class ShopService
    {

        private readonly ILogger _logger;
        private readonly IShopRepositoryFacade _repository;

        private readonly IShopNameMapper _shopNameMapper;
        private readonly IShopOverviewMapper _shopOverviewMapper;

        public ShopService(ILogger logger, IShopRepositoryFacade repository,
            IWebInterfaceMapperProvider mapperProvider)
        {
            _logger = logger;
            _repository = repository;
            _shopNameMapper = mapperProvider.ShopNameMapper;
            _shopOverviewMapper = mapperProvider.ShopOverviewMapper;
        }


        // As ShopNameDto
        public List<ShopNameDto> GetShops()
        {
            return _repository.GetAll().Select(_shopNameMapper.Map).ToList();
        }

        // As ShopOverviewDto

        public ShopOverviewDto? GetShop(int id)
        {
            var model = _repository.GetModel(id);
            return model != null ? _shopOverviewMapper.Map(model) : null;
        }


        public void CreateShop(ShopNameDto shop)
        {
            ShopDto shopDto = new(default, shop.Name, []);
            _repository.Create(shopDto);

        }


        public bool ChangeShopName(int id, string shopName)
        {
            var model = _repository.GetModel(id);
            if (model != null)
            {
                ShopDto updated = new(model.Id, shopName, model.Merches);
                return _repository.Update(updated);
            }
            return false;
        }


        public bool DeleteShop(int id)
        {
            return _repository.Delete(id);
        }


    }
}
