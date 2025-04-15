using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DTOModels.ForAPI.Shop;

namespace PriceTracker.Models.Services.Mapping.MicroMappers
{
    public class ShopToDtoMapper: IShopToDtoMapper
    {
        public ShopNameDto ToShopName(ShopModel model)
        {
            return new ShopNameDto(model.Name, model.Id);

        }
        public ShopOverviewDto ToShopOverview(ShopModel model, string merchesUrl)
        {
            return new(model.Name, merchesUrl, model.Id);
        }

    }
}
