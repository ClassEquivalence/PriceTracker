using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.CoordinationLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.UpsertionInvocationLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;
using PriceTracker.Modules.MerchDataMaintenance.Facade;
using PriceTracker.Modules.WebInterface.API.Mapping.MapperProvider;
using PriceTracker.Modules.WebInterface.API.Routing;

namespace PriceTracker.Core.Utils.DependencyInjection
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
        }

        public static void InjectMerchDataMaintenanceDependencies(
            IServiceCollection collection)
        {
            MerchDataMaintenanceInjector.InjectMerchDataProviderDependencies(collection);
        }

    }
}
