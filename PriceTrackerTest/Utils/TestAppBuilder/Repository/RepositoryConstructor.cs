using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository;
using PriceTracker.Modules.Repository;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Entities.Domain.MerchPriceHistory;
using PriceTracker.Modules.Repository.Entities.Domain.ShopSpecific;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.Repository.Facade.Implementations;
using PriceTracker.Modules.Repository.Mapping.Domain;
using PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.Repository.Repositories.Base;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel.MerchRepository;
using PriceTracker.Modules.Repository.Repositories.Domain.EntityLevel;
using PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink;
using PriceTracker.Modules.WebInterface.Adapters.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTrackerTest.Utils.TestAppBuilder.Repository
{
    public class RepositoryConstructor<DbContextType>
        where DbContextType: PriceTrackerContext
    {
        public RepositoryConstructor()
        {

        }

        public IServiceCollection AddRepository(IServiceCollection services)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = "TestDatabase",
                Mode = SqliteOpenMode.Memory,
                Cache = SqliteCacheMode.Shared
            };
            var connectionString = connectionStringBuilder.ToString();

            // Пока эта переменная жива, база данных существует в памяти
            var connection = new SqliteConnection(connectionString);
            connection.Open();

            services.AddSingleton(connection);
            services.AddDbContextFactory<DbContextType>();
            services.AddSingleton<IDbContextFactory<PriceTrackerContext>, 
                TestDbContextFactory<PriceTrackerContext, DbContextType>>();

            services = AddEntityRepositories(services);
            services = AddMappers(services);
            services = AddRepositories(services);
            services = AddFacades(services);
            services = AddAdapters(services);

            services.AddSingleton<RepositoryInitializer>();

            return services;
        }
        private static IServiceCollection AddEntityRepositories(IServiceCollection services)
        // completed (parent = repositories)
        {
            services.AddSingleton<IEntityRepository<ShopEntity>,
                ShopEntityRepository>();

            services.AddSingleton<IEntityRepository<TimestampedPriceEntity>,
                TimestampedPriceEntityRepository>();
            services.AddSingleton<IEntityRepository<MerchPriceHistoryEntity>,
                PriceHistoryEntityRepository>();
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
            services.AddSingleton<IMerchCoreToEntityMapper<MerchDto, MerchEntity>,
                MerchMapper>();

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
            services.AddSingleton<CitilinkExtractorStorageStateRepository>();
            services.AddSingleton<CitilinkExtractionStateRepository>();
            services.AddSingleton<ShopRepository>(); //s
            services.AddSingleton<TimestampedPriceRepository>();
            services.AddSingleton<PriceHistoryRepository>();
            services.AddSingleton<CitilinkMerchRepository>();

            services.AddSingleton<IMerchSubtypeRepositoryAdapter, CitilinkMerchRepositoryAdapter>();
            services.AddSingleton<MerchRepository>();

            return services;
        }

        private static IServiceCollection AddAdapters(IServiceCollection services) //completed
        {
            services.AddSingleton<ICitilinkCatalogTreeRepositoryAdapter,
                NewCitilinkCatalogTreeRepositoryAdapter>();
            services.AddSingleton<IMerchRepositoryAdapter, MerchRepositoryAdapter>();
            return services;
        }

        private static IServiceCollection AddFacades(IServiceCollection services)
        {
            services.AddSingleton<IMerchRepositoryFacade, MerchRepositoryFacade>();
            services.AddSingleton<ICitilinkMiscellaneousRepositoryFacade, CitilinkMiscellaneousRepositoryFacade>();
            services.AddSingleton<IShopRepositoryFacade, ShopRepositoryFacade>();
            services.AddSingleton<IShopSelectorFacade, ShopSelectorFacade>();

            services.AddSingleton<ITimestampedPriceRepositoryFacade, TimestampedPriceRepositoryFacade>();
            services.AddSingleton<IPriceHistoryRepositoryFacade, PriceHistoryRepositoryFacade>();
            services.AddSingleton<ICitilinkMerchRepositoryFacade, CitilinkMerchRepositoryFacade>();
            return services;
        }
    }
}
