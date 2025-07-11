using Microsoft.Playwright;
using PriceTracker.Modules.MerchDataUpserter.Core.Models.ForParsing;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Utils.ScrapingServices.HttpClients.Browser;
using PriceTracker.Modules.Repository.Facade;
using PriceTracker.Modules.Repository.Facade.Citilink;


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
        private readonly IRepositoryFacade _repository;
        private readonly ILogger _logger;

        public MerchDataProviderFacade(IRepositoryFacade repository, ILogger<Program> logger)
        {
            _logger = logger;

            IExtractionExecutionStateProvider<CitilinkParsingExecutionState> executionStateProvider
                = repository;
            _repository = repository;
            

            CitilinkMerchDataUpserter consumer = new(repository, repository.GetCitilinkShop(),
                _logger);

            var browser = Playwright.CreateAsync().Result.Chromium.LaunchAsync().Result;
            BrowserAdapter browserAdapter = new(browser, Configs.HeadlessBrowserDelayRange,
                _logger);

            string CitilinkStorageState = ((ICitilinkMiscellaneousRepositoryFacade)_repository).
                GetExtractorStorageState();
            GUICitilinkExtractor extractor =
                new(browserAdapter, Configs.MaxPageRequestsPerTime, _logger, storageState: 
                CitilinkStorageState);

            ScheduledCitilinkMerchUpserter
                scheduledCitilinkMerchUpserter = new(consumer, extractor, Configs.PriceUpdatePeriod,
                DateTime.Now, executionStateProvider.Provide(), repository);

            _scheduledUpserter = new([scheduledCitilinkMerchUpserter]);

        }

        public async Task ProcessMerchUpsertion()
        {
            _logger.LogTrace("Запущен процесс upsert'а товаров.");
            await _scheduledUpserter.ProcessUpsertion();
        }

        public async Task OnShutdownAsync()
        {
            await _scheduledUpserter.OnShutdown();
        }
    }
}
