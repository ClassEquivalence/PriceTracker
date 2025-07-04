using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Shop;
using PriceTracker.Modules.WebInterface.Routing;

namespace PriceTracker.Modules.WebInterface.Mapping.Shop
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
