using PriceTracker.Modules.MerchDataProvider;
using PriceTracker.Modules.Repository.Facade;
using PriceTracker.Modules.WebInterface.Mapping.Merch;
using PriceTracker.Modules.WebInterface.Mapping.Shop;
using PriceTracker.Modules.WebInterface.Routing;
using PriceTracker.Modules.WebInterface.Services.InterfaceServices;
using PriceTracker.Modules.WebInterface.Services.MerchService;
using PriceTracker.Modules.WebInterface.Services.MerchService.Citilink;
using PriceTracker.Modules.WebInterface.Services.ShopService;

namespace PriceTracker.Core.Utils
{
    public static class DependencyInjector
    {


        public static void InjectRepositoryDependencies(
            IServiceCollection collection)
        {
            collection.AddSingleton<IRepositoryFacade, RepositoryFacade>();
            collection.AddSingleton<IShopRepositoryFacade>(s => s.GetRequiredService<IRepositoryFacade>());
            collection.AddSingleton<ICitilinkMerchRepositoryFacade>(s =>
            s.GetRequiredService<IRepositoryFacade>());
        }

        public static void InjectWebInterfaceDependencies(
            IServiceCollection collection)
        {
            InjectAdminApiServiceDependencies(collection);
            collection.AddSingleton<AdminAPIService>();
        }
        
        public static void InjectMerchDataProviderDependencies(
            IServiceCollection collection)
        {
            collection.AddSingleton<IMerchDataProviderFacade, MerchDataProviderFacade>();
        }

        private static void InjectAdminApiServiceDependencies(
            IServiceCollection collection)
        {
            collection.AddSingleton<AdminAPIService>();
            /*
             
            (IDetailedMerchDtoMapper detailedMerchDtoMapper,
            IOverviewMerchDtoMapper overviewMerchDtoMapper, IMerchService merchService,
            IShopService shopService, IShopNameMapper shopNameMapper, 
            IShopOverviewMapper shopOverviewMapper, ICitilinkMerchService 
            citilinkMerchService) 

             */
            collection.AddSingleton<IDetailedMerchDtoMapper, DetailedMerchDtoMapper>();
            collection.AddSingleton<IOverviewMerchDtoMapper, OverviewMerchDtoMapper>();
            collection.AddSingleton<IMerchService, MerchService>();
            collection.AddSingleton<IShopService, ShopService>();
            collection.AddSingleton<IShopNameMapper, ShopNameMapper>();
            collection.AddSingleton<IShopOverviewMapper, ShopOverviewMapper>();

            collection.AddSingleton<ICitilinkMerchService, CitilinkMerchService>();
            collection.AddSingleton<APILinkBuilder>();
            // Нужно ли инжектить LinkGenerator?
        }
    }
}
