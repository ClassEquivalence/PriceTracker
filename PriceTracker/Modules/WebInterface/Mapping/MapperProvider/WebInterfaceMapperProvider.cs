using PriceTracker.Modules.WebInterface.Mapping.Merch;
using PriceTracker.Modules.WebInterface.Mapping.Shop;
using PriceTracker.Modules.WebInterface.Routing;

namespace PriceTracker.Modules.WebInterface.Mapping.MapperProvider
{
    public class WebInterfaceMapperProvider : IWebInterfaceMapperProvider
    {
        public WebInterfaceMapperProvider(APIRouteLinkBuilder routeLinkBuilder)
        {
            ShopNameMapper = new ShopNameMapper();
            ShopOverviewMapper = new ShopOverviewMapper(routeLinkBuilder);
            DetailedMerchDtoMapper = new DetailedMerchDtoMapper();
            OverviewMerchDtoMapper = new OverviewMerchDtoMapper();
        }

        public IShopNameMapper ShopNameMapper { get; init; }

        public IShopOverviewMapper ShopOverviewMapper { get; init; }

        public IDetailedMerchDtoMapper DetailedMerchDtoMapper { get; init; }

        public IOverviewMerchDtoMapper OverviewMerchDtoMapper { get; init; }
    }
}
