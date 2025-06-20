using Microsoft.Playwright;
using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Models.DomainModels.ShopSpecificModels.Citilink;
using PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine;
using PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine.ScrapingServices.HttpClients.Browser;
using PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataProvider.MerchDataExtraction.MerchExtractionEngine;
using PriceTracker.Modules.MerchDataProvider.Models.ForParsing;
using PriceTracker.Modules.MerchDataProvider.Upsertion;
using PriceTracker.Modules.MerchDataProvider.Upsertion.ScheduledTriggerer;
using PriceTracker.Modules.Repository.Facade;
using System.Runtime.CompilerServices;


/*
 Сделать передачу в репозиторий состояния выполнения
 Да и вообще зарефакторить весь модуль надо бы.
 */
namespace PriceTracker.Modules.MerchDataProvider
{
    /// <summary>
    /// Точка взаимодействия с модулем, и модуля - с внешним миром.
    /// </summary>
    public abstract class MerchDataProviderFacade: IMerchDataProviderFacade
    {
        private readonly ScheduledMerchUpserter _scheduledUpserter;
        public MerchDataProviderFacade(IRepositoryFacade repository)
        {
            
            CitilinkMerchDataUpserter consumer = new(repository, repository.GetCitilinkShop());

            var browser = Playwright.CreateAsync().Result.Chromium.LaunchAsync().Result;
            BrowserAdapter browserAdapter = new(browser, Configs.HeadlessBrowserDelayRange);
            CitilinkScraper scraper = new(browserAdapter);
            IMerchDataExtractor<CitilinkMerchParsingDto, CitilinkParsingExecutionState>
                extractor = new GUICitilinkExtractor(scraper);

            MerchExtractionAgent<CitilinkMerchParsingDto, CitilinkParsingExecutionState>
                citilinkAgent = new(consumer, extractor, repository);
            MerchExtractionCoordinator coordinator = new([citilinkAgent]);

            ScheduledMerchUpserter scheduledUpserter = new(coordinator, 
                repository.GetLastTimeExtractionProcessHappened(),
                repository);

            _scheduledUpserter = scheduledUpserter;

        }

        public async Task ProcessMerchUpsertion()
        {
            await _scheduledUpserter.ProcessScheduledExtraction();
        }
    }
}
