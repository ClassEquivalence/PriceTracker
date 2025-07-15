using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.API.DTOModels.Shop;
using PriceTracker.Modules.WebInterface.API.Routing;

namespace PriceTracker.Modules.WebInterface.API.Mapping.Shop
{
    public class ShopOverviewMapper : IShopOverviewMapper
    {
        private readonly APIRouteLinkBuilder _linkBuilder;
        public ShopOverviewMapper(APIRouteLinkBuilder linkBuilder)
        {
            _linkBuilder = linkBuilder;
        }
        public ShopOverviewDto Map(ShopDto dto)
        {
            return new ShopOverviewDto(dto.Name,
                _linkBuilder.GetShopMerchesPath(dto.Id), dto.Id);
        }
    }
}
