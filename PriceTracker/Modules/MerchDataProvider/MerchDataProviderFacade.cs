using Microsoft.Playwright;
using PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine;
using PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine.ScrapingServices.HttpClients.Browser;
using PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataProvider.MerchDataExtraction.MerchExtractionEngine;
using PriceTracker.Modules.MerchDataProvider.Models.ForParsing;
using PriceTracker.Modules.MerchDataProvider.Upsertion;
using PriceTracker.Modules.MerchDataProvider.Upsertion.ScheduledTriggerer;
using PriceTracker.Modules.Repository.Facade;


/*
 Сделать передачу в репозиторий состояния выполнения
 Да и вообще зарефакторить весь модуль надо бы.
 */
namespace PriceTracker.Modules.MerchDataProvider
{
    /// <summary>
    /// Точка взаимодействия с модулем, и модуля - с внешним миром.
    /// </summary>
    public class MerchDataProviderFacade : IMerchDataProviderFacade
    {
        private readonly ScheduledMerchUpserter _scheduledUpserter;
        private readonly ILogger _logger;
        public MerchDataProviderFacade(IRepositoryFacade repository, ILogger<Program> logger)
        {
            _logger = logger;


            CitilinkMerchDataUpserter consumer = new(repository, repository.GetCitilinkShop(),
                _logger);

            var browser = Playwright.CreateAsync().Result.Chromium.LaunchAsync().Result;
            BrowserAdapter browserAdapter = new(browser, Configs.HeadlessBrowserDelayRange,
                _logger);
            CitilinkScraper scraper = new(browserAdapter, logger: _logger);
            CitilinkMerchParser citilinkMerchParser = new(scraper, logger);
            IMerchDataExtractor<CitilinkMerchParsingDto, CitilinkParsingExecutionState>
                extractor = new GUICitilinkExtractor(citilinkMerchParser, _logger);

            MerchExtractionAgent<CitilinkMerchParsingDto, CitilinkParsingExecutionState>
                citilinkAgent = new(consumer, extractor, repository, _logger);
            MerchExtractionCoordinator coordinator = new([citilinkAgent], _logger);

            ScheduledMerchUpserter scheduledUpserter = new(coordinator,
                repository.GetLastTimeExtractionProcessHappened(),
                repository, _logger);

            _scheduledUpserter = scheduledUpserter;

        }

        public async Task ProcessMerchUpsertion()
        {
            _logger.LogTrace("Запущен процесс upsert'а товаров.");
            await _scheduledUpserter.ProcessScheduledExtraction();
        }
    }
}
