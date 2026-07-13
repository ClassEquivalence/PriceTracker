using Microsoft.Extensions.Logging;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.CoordinationLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Inner.CatalogTreeMaintainer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Pipeline;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Upsertion;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner.Parsers;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Pipeline;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;
using PriceTracker.Modules.MerchDataMaintenance.Facade;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.Repository.Repositories.Domain.CoreDtoLevel;

namespace PriceTracker.Core.Utils.DependencyInjection
{
    public static class MerchDataMaintenanceInjector
    {
        public static void InjectMerchDataProviderDependencies(
            IServiceCollection collection)
        {
            //TODO: CitilinkUpsertionOptions - заDIшить нормально и переосмыслить мб?
            // TODO: также - дальше DIшить уже ток репозиторий и смежные слои?



            //public CitilinkCatalogTreeMaintainer(ICitilinkCatalogTreeRepositoryAdapter treeRepository)



            //public InnerCitilinkMerchUpserter(ICitilinkMerchRepositoryFacade repository,
            //IShopRepositoryFacade shopRepository, int upsertablePortionSize = 100,
            //ILogger? logger = null)


            //public CitilinkInnerDataAugmenter(IInnerCitilinkMerchUpserter merchUpserter,
            //ICitilinkCatalogTreeMaintainer treeMaintainer)
            collection.AddSingleton<IInnerCitilinkMerchUpserter, InnerCitilinkMerchUpserter>();
            collection.AddSingleton<ICitilinkCatalogTreeMaintainer, CitilinkCatalogTreeMaintainer>();

            /*
             public InnerCitilinkParser(ICitilinkParsableClassifier classifier,
            ICitilinkCatalogsCatalogParser catalogsCatalogParser,
            ICitilinkMerchCatalogParser merchCatalogParser,
            ICitilinkRootCatalogParser rootCatalogParser)
             */
            collection.AddSingleton<ICitilinkParsableClassifier, CitilinkParsablesClassifier>();
            collection.AddSingleton<ICitilinkCatalogsCatalogParser,  CitilinkCatalogsCatalogParser>();
            collection.AddSingleton<ICitilinkMerchCatalogParser, CitilinkMerchCatalogParser>();
            collection.AddSingleton<ICitilinkRootCatalogParser, CitilinkRootCatalogParser>();


            //public NewCitilinkScraper(HttpClient client, ILogger? logger = null)

            // сигнлтоном ли или можно scoped? вызывается в:
            // NewCitilinkScraper
            collection.AddSingleton<HttpClient>();

            //public CitilinkScraper(IInnerCitilinkScraper innerScraper,
            //ICitilinkScrapeAssignmentFactory assignmentFactory)
            collection.AddSingleton<IInnerCitilinkScraper, NewCitilinkScraper>();
            collection.AddSingleton<ICitilinkScrapeAssignmentFactory, 
                CitilinkScrapeAssignmentFactory>();

            //public NewCitilinkOuterParser(ICitilinkScraper scraper, IInnerCitilinkParser
            //parser)
            collection.AddSingleton<ICitilinkScraper, CitilinkScraper>();
            collection.AddSingleton<IInnerCitilinkParser, InnerCitilinkParser>();

            // CitilinkDataAugmenter: ICitilinkParser, CitilinkInnerDataAugmenter
            collection.AddSingleton<CitilinkInnerDataAugmenter>();
            collection.AddSingleton<ICitilinkParser, NewCitilinkOuterParser>();
            collection.AddSingleton<ICitilinkDataAugmenter, CitilinkDataAugmenter>();

            collection.AddSingleton<CitilinkEnforcersSharedContext>();
            collection.AddSingleton<ICitilinkPolicyEnforcersContextfulProvider,
                CitilinkPolicyEnforcersContextfulProvider>();
            collection.AddSingleton<ICitilinkUpsertionInvoker, CitilinkUpsertionInvoker>();
        }
    }
}
