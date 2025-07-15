using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.API.DTOModels.Shop;

namespace PriceTracker.Modules.WebInterface.API.Mapping.Shop
{
    public interface IShopOverviewMapper : ICoreToWebInterfaceMapper<ShopDto,
        ShopOverviewDto>
    {

    }
}
