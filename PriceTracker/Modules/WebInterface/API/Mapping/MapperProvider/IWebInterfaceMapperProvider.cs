using PriceTracker.Modules.WebInterface.API.Mapping.Merch;
using PriceTracker.Modules.WebInterface.API.Mapping.Shop;

namespace PriceTracker.Modules.WebInterface.API.Mapping.MapperProvider
{
    public interface IWebInterfaceMapperProvider
    {
        IShopNameMapper ShopNameMapper { get; }
        IShopOverviewMapper ShopOverviewMapper { get; }
        IDetailedMerchDtoMapper DetailedMerchDtoMapper { get; }
        IOverviewMerchDtoMapper OverviewMerchDtoMapper { get; }
    }
}
