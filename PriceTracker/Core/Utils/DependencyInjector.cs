using PriceTracker.Modules.MerchDataProvider;
using PriceTracker.Modules.Repository.Facade;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.WebInterface.Mapping.MapperProvider;
using PriceTracker.Modules.WebInterface.Routing;


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
            collection.AddSingleton<IWebInterfaceMapperProvider, WebInterfaceMapperProvider>();
            collection.AddSingleton<APIRouteLinkBuilder>();
        }

        public static void InjectMerchDataProviderDependencies(
            IServiceCollection collection)
        {
            collection.AddSingleton<IMerchDataProviderFacade, MerchDataProviderFacade>();
        }

    }
}
