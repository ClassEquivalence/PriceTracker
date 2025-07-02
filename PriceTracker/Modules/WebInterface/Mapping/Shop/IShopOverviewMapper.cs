using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Shop;

namespace PriceTracker.Modules.WebInterface.Mapping.Shop
{
    public interface IShopOverviewMapper : ICoreToWebInterfaceMapper<ShopDto,
        ShopOverviewDto>
    {

    }
}
