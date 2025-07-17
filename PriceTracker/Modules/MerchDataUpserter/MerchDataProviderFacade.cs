using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
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

            IExtractionExecutionStateProvider<CitilinkExtractionStateDto> executionStateProvider
                = repository;
            _repository = repository;


            CitilinkMerchDataUpserter consumer = new(repository, repository.GetCitilinkShop(),
                _logger);

            var browser = Playwright.CreateAsync().Result.Chromium.LaunchAsync().Result;
            BrowserAdapter browserAdapter = new(browser, Configs.HeadlessBrowserDelayRange,
                _logger);

            var CitilinkStorageState = ((ICitilinkMiscellaneousRepositoryFacade)_repository).
                GetExtractorStorageState();
            GUICitilinkExtractor extractor = 
                new(browserAdapter, Configs.MaxPageRequestsPerTime, _logger, storageState:
                CitilinkStorageState?.StorageState);

            var citilinkExtractionStateDto = executionStateProvider.Provide();
            CitilinkParsingExecutionState citilinkExtractionState = new(citilinkExtractionStateDto.CurrentCatalogUrl,
                citilinkExtractionStateDto.CatalogPageNumber, citilinkExtractionStateDto.IsCompleted);

            ScheduledCitilinkMerchUpserter
                scheduledCitilinkMerchUpserter = new(consumer, extractor, Configs.PriceUpdatePeriod,
                DateTime.Now, citilinkExtractionState, repository);

            _scheduledUpserter = new([scheduledCitilinkMerchUpserter]);

        }

        public async Task ProcessMerchUpsertion()
        {
            _logger.LogTrace("Запущен процесс upsert'а товаров.");
            try
            {
                await _scheduledUpserter.ProcessUpsertion();
            }
            catch(DbUpdateException ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
            }
            /*
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            */
        }

        public async Task OnShutdownAsync()
        {
            await _scheduledUpserter.OnShutdown();

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(MerchDataProviderFacade)}: shutdown" +
                $"process started.");
            await OnShutdownAsync();
            _logger.LogInformation($"{nameof(MerchDataProviderFacade)}: shutdown" +
               $"process completed.");
        }
    }
}
