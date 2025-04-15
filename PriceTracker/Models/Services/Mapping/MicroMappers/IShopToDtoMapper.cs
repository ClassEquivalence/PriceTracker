using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DTOModels.ForAPI.Shop;

namespace PriceTracker.Models.Services.Mapping.MicroMappers
{
    public interface IShopToDtoMapper
    {
        public ShopNameDto ToShopName(ShopModel model);
        public ShopOverviewDto ToShopOverview(ShopModel model, string merchesUrl);
    }
}
