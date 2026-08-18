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
