using PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository;
using PriceTracker.Modules.Repository;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.Repository.Facade.Implementations;
using PriceTracker.Modules.Repository.Mapping.Domain;
using PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink;
using PriceTracker.Modules.WebInterface.Adapters.Repository;

namespace PriceTracker.Core.App
{
    public class RepositoryConstructor
    {
        public RepositoryConstructor()
        {

        }
        public IServiceCollection AddRepository(IServiceCollection services)
        {
            services.AddDbContextFactory<PriceTrackerContext>();
            services = AddEntityRepositories(services);
            services = AddMappers(services);
            services = AddRepositories(services);
            services = AddFacades(services);
            services = AddAdapters(services);
            services.AddSingleton<RepositoryInitializer>();
            return services;
        }
        private static IServiceCollection AddEntityRepositories(IServiceCollection services)
        {
            return services;
        }
        private static IServiceCollection AddMappers(IServiceCollection services)
        {
            services.AddSingleton<ITimestampedPriceMapper,
                TimestampedPriceMapper>();
            services.AddSingleton<IPriceHistoryMapper,
                PriceHistoryMapper>();
            services.AddSingleton<ICitilinkMerchCoreToEntityMapper,
                CitilinkMerchCoreToEntityMapper>();
            return services;
        }
        private static IServiceCollection AddRepositories(IServiceCollection services)
        {
            return services;
        }
        private static IServiceCollection AddAdapters(IServiceCollection services)
        {
            services.AddSingleton<ICitilinkCatalogTreeRepositoryAdapter,
                NewCitilinkCatalogTreeRepositoryAdapter>();
            services.AddSingleton<IMerchRepositoryAdapter, MerchRepositoryAdapter>();
            services.AddSingleton<IMerchDataMaintenanceRepositoryAdapter,
                MerchDataMaintenanceRepositoryAdapter>();
            return services;
        }
        private static IServiceCollection AddFacades(IServiceCollection services)
        {
            services.AddSingleton<IShopRepositoryFacade, ShopRepositoryFacade>();
            return services;
        }
    }
}
