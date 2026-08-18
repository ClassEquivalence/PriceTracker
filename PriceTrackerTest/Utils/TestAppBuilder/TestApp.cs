
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Pipeline;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;
using PriceTracker.Modules.Repository;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;
using PriceTracker.Modules.WebInterface.Adapters.Repository;
using PriceTrackerTest.Utils.TestAppBuilder.Repository;

namespace PriceTrackerTest.Utils.TestAppBuilder
{
    public class TestApp
    {
        private readonly IServiceProvider _appServiceProvider;
        public TestApp(IServiceProvider appServiceProvider)
        {
            _appServiceProvider = appServiceProvider;
        }
        public ICitilinkUpsertionInvoker ProvideUpsertionInvoker()
        {
            return ProvideService<ICitilinkUpsertionInvoker>();
        }
        public ShopRepository ProvideShopRepository()
        {
            return ProvideService<ShopRepository>();
        }
        public ICitilinkParsableClassifier ProvideParsablesClassifier()
        {
            return ProvideService<ICitilinkParsableClassifier>();
        }
        public ICitilinkScraper ProvideScraper()
        {
            return ProvideService<ICitilinkScraper>();
        }
        public ILogger ProvideLogger()
        {
            return ProvideService<ILogger>();
        }

        public ICitilinkMiscellaneousRepositoryFacade ProvideMiscRepository()
        {
            return ProvideService<ICitilinkMiscellaneousRepositoryFacade>();
        }

        public ICitilinkCatalogTreeRepositoryAdapter ProvideCatalogTreeRepository()
        {
            return ProvideService<ICitilinkCatalogTreeRepositoryAdapter>();
        }
        public ICitilinkMerchRepositoryFacade ProvideMerchRepository()
        {
            return ProvideService<ICitilinkMerchRepositoryFacade>();
        }
        public IMerchRepositoryAdapter ProvideMerchRepositoryAdapter()
        {
            return ProvideService<IMerchRepositoryAdapter>();
        }
        public IDbContextFactory<PriceTrackerTestContext> ProvideDbContextFactory()
        {
            return ProvideService<IDbContextFactory<PriceTrackerTestContext>>();
        }


        public RepositoryInitializer ProvideRepositoryInitializer()
        {
            return ProvideService<RepositoryInitializer>();
        }

        public void Initialize()
        {
            var initializer = ProvideRepositoryInitializer();
            initializer.EnsureInitialized();
        }

        private T ProvideService<T>()
        {
            var service = _appServiceProvider.GetService<T>();
            if (service == null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                return service;
            }
        }
    }
}
