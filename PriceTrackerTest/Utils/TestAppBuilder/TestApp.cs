using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Pipeline;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;
using PriceTracker.Modules.Repository;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.WebInterface.Adapters.Repository;


namespace PriceTrackerTest.Utils.TestAppBuilder
{
    public class TestApp
    {
        private readonly IServiceProvider _appServiceProvider;
        public TestApp(IServiceProvider appServiceProvider)
        {
            _appServiceProvider = appServiceProvider;
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
        public ICitilinkCatalogTreeRepositoryAdapter ProvideCatalogTreeRepository()
        {
            return ProvideService<ICitilinkCatalogTreeRepositoryAdapter>();
        }
        public IMerchRepositoryAdapter ProvideMerchRepositoryAdapter()
        {
            return ProvideService<IMerchRepositoryAdapter>();
        }
        public IDbContextFactory<PriceTrackerContext> ProvideDbContextFactory()
        {
            return ProvideService<IDbContextFactory<PriceTrackerContext>>();
        }


        public RepositoryInitializer ProvideRepositoryInitializer()
        {
            return ProvideService<RepositoryInitializer>();
        }

        public CitilinkPartialCycleExecutor ProvideCycleExecutor()
        {
            return ProvideService<CitilinkPartialCycleExecutor>();
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
