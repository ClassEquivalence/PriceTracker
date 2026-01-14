using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.CoordinationLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.UpsertionInvocationLayer;
using PriceTracker.Modules.MerchDataMaintenance.Facade;
using PriceTracker.Modules.Repository;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Facade;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.WebInterface.API.Mapping.MapperProvider;
using PriceTracker.Modules.WebInterface.API.Routing;

namespace PriceTracker.Core.Utils
{
    public static class DependencyInjector
    {
        public static void InjectRepositoryDependencies(
            IServiceCollection collection)
        {
            collection.AddRepository();
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


            collection.AddSingleton<UpsertionService>();
            collection.AddSingleton<ScheduledMerchUpserter, ScheduledCitilinkMerchUpserter>();

            collection.AddSingleton<IMerchDataProviderFacade, MerchDataProviderFacade>();
            collection.AddSingleton<IHostedService>(sp => sp.
            GetRequiredService<IMerchDataProviderFacade>());
        }

    }
}
