using Microsoft.Playwright;
using PriceTracker.Modules.MerchDataUpserter.Core.Models.ForParsing;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Utils.ScrapingServices.HttpClients.Browser;
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
        private readonly UpsertionService _scheduledUpserter;
        private readonly ILogger _logger;
        public MerchDataProviderFacade(IRepositoryFacade repository, ILogger<Program> logger)
        {
            _logger = logger;

            IExtractionExecutionStateProvider<CitilinkParsingExecutionState> executionStateProvider
                = repository;

            CitilinkMerchDataUpserter consumer = new(repository, repository.GetCitilinkShop(),
                _logger);

            var browser = Playwright.CreateAsync().Result.Chromium.LaunchAsync().Result;
            BrowserAdapter browserAdapter = new(browser, Configs.HeadlessBrowserDelayRange,
                _logger);
            IMerchDataExtractor<CitilinkMerchParsingDto, CitilinkParsingExecutionState>
                extractor = new GUICitilinkExtractor(browserAdapter, Configs.MaxPageRequestsPerTime, _logger);

            ScheduledMerchUpserter<CitilinkMerchParsingDto, CitilinkParsingExecutionState>
                scheduledCitilinkMerchUpserter = new(consumer, extractor, Configs.PriceUpdatePeriod,
                DateTime.Now, executionStateProvider.Provide());

            _scheduledUpserter = new([scheduledCitilinkMerchUpserter]);

        }

        public async Task ProcessMerchUpsertion()
        {
            _logger.LogTrace("Запущен процесс upsert'а товаров.");
            await _scheduledUpserter.ProcessUpsertion();
        }
    }
}
