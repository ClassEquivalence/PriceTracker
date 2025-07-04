using PriceTracker.Modules.WebInterface.Mapping.Merch;
using PriceTracker.Modules.WebInterface.Mapping.Shop;

namespace PriceTracker.Modules.WebInterface.Mapping.MapperProvider
{
    public interface IWebInterfaceMapperProvider
    {
        IShopNameMapper ShopNameMapper { get; }
        IShopOverviewMapper ShopOverviewMapper { get; }
        IDetailedMerchDtoMapper DetailedMerchDtoMapper { get; }
        IOverviewMerchDtoMapper OverviewMerchDtoMapper { get; }
    }
}
