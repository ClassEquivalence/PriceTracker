using PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository;
using PriceTracker.Modules.Repository;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Entities.Domain.ShopSpecific;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.Repository.Facade.Implementations;
using PriceTracker.Modules.Repository.Mapping.Domain;
using PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.Repository.Repositories.Base;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;
using PriceTracker.Modules.Repository.Repositories.Domain.EntityLevel;
using PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink;
using PriceTracker.Modules.WebInterface.Adapters.Repository;

namespace PriceTracker.Core.Utils.DependencyInjection
{

    public static class RepositoryDependencyInjection
    {

        private static IServiceCollection AddEntityRepositories(IServiceCollection services)
        // completed (parent = repositories)
        {
            services.AddSingleton<IEntityRepository<ShopEntity>,
                ShopEntityRepository>();
            services.AddSingleton<CitilinkMerchEntityRepository>();

            services.AddSingleton<IEntityRepository<CitilinkMerchEntity>>(s =>
            s.GetService<CitilinkMerchEntityRepository>());

            return services;

        }

        private static IServiceCollection AddMappers(IServiceCollection services) //completed?
        {

            services.AddSingleton<ITimestampedPriceMapper,
                TimestampedPriceMapper>();
            services.AddSingleton<IPriceHistoryMapper,
                PriceHistoryMapper>();

            services.AddSingleton<ICitilinkCatalogUrlBranchMapper, CitilinkCatalogUrlBranchMapper>();
            services.AddSingleton<ICitilinkCatalogUrlTreeMapper, CitilinkCatalogUrlTreeMapper>();

            services.AddSingleton<ICitilinkExtractorStorageStateMapper,
                CitilinkExtractorStorageStateMapper>();
            services.AddSingleton<ICitilinkExtractionStateMapper,
                CitilinkExtractionStateMapper>();
            services.AddSingleton<IShopMapper,
                ShopCoreToEntityMapper>();

            services.AddSingleton<ICitilinkMerchCoreToEntityMapper,
                CitilinkMerchCoreToEntityMapper>();


            return services;

        }

        private static IServiceCollection AddRepositories(IServiceCollection services) //completed
        {
            services.AddSingleton<ShopRepository>(); //s
            services.AddSingleton<CitilinkMerchRepository>();
            return services;
        }

        private static IServiceCollection AddAdapters(IServiceCollection services) //completed
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

        public static IServiceCollection AddRepository(this IServiceCollection services)
        {

            //public CitilinkCatalogTreeMaintainer(ICitilinkCatalogTreeRepositoryAdapter treeRepository)

            //public InnerCitilinkMerchUpserter(ICitilinkMerchRepositoryFacade repository,
            //ShopRepository shopRepository, int upsertablePortionSize = 100,
            //ILogger? logger = null)

            services.AddDbContextFactory<PriceTrackerContext>();

            services = AddEntityRepositories(services);
            services = AddMappers(services);
            services = AddRepositories(services);
            services = AddFacades(services);
            services = AddAdapters(services);

            services.AddSingleton<RepositoryInitializer>();

            return services;
        }
    }
}
