using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using PriceTracker.Core.Configuration.ProvidedWithDI;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Utils.ScrapingServices.HttpClients.Browser;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;


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
        private readonly MerchUpsertionOptions _options;

        public MerchDataProviderFacade(IRepositoryFacade repository, ILogger<Program> logger,
            IOptions<MerchUpsertionOptions> options)
        {
            _options = options.Value;
            var citilinkOptions = _options.CitilinkUpsertionOptions;
            _logger = logger;

            IExtractionExecutionStateProvider<CitilinkExtractionStateDto> executionStateProvider
                = repository;
            _repository = repository;


            CitilinkMerchDataUpserter consumer = new(repository, repository.GetCitilinkShop(),
                logger: _logger);

            var browser = Playwright.CreateAsync().Result.Chromium.LaunchAsync().Result;
            BrowserAdapter browserAdapter = new(browser, (citilinkOptions.HeadlessBrowserMinDelay,
                citilinkOptions.HeadlessBrowserMaxDelay), _options.UserAgent,
                _logger);

            var CitilinkStorageState = ((ICitilinkMiscellaneousRepositoryFacade)_repository).
                GetExtractorStorageState();
            GUICitilinkExtractor extractor =
                new(browserAdapter, (citilinkOptions.MaxPageRequestsPerTime, 
                TimeSpan.FromHours(citilinkOptions.MinCooldownForPageRequests)), new(), citilinkOptions,
                _options.UserAgent, _logger, storageState: CitilinkStorageState?.StorageState);

            ScheduledCitilinkMerchUpserter
                scheduledCitilinkMerchUpserter = new(consumer, extractor, TimeSpan.
                FromDays(citilinkOptions.CitilinkPriceUpdatePeriod),
                DateTime.Now, repository, _logger, TimeSpan.FromHours(citilinkOptions.MinCooldownForPageRequests));

            _scheduledUpserter = new([scheduledCitilinkMerchUpserter]);

        }

        public async Task ProcessMerchUpsertion()
        {
            _logger.LogInformation($"{nameof(MerchDataProviderFacade)}: " +
                $"Запущен процесс upsert'а товаров.");


            try
            {
                await _scheduledUpserter.ProcessUpsertion();
            }
            catch (DbUpdateException ex)
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
            if (_options.UpsertionActive)
            {
                ProcessMerchUpsertion();
            }
            else
            {
                _logger?.LogInformation($"{nameof(MerchDataProviderFacade)}: Merch upsertion" +
                $" inactive.");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger?.LogInformation($"{nameof(MerchDataProviderFacade)}: shutdown" +
                $"process started.");
            if (_options.UpsertionActive)
            {
                
                await OnShutdownAsync();
                _logger?.LogInformation($"{nameof(MerchDataProviderFacade)}: shutdown" +
                   $"process completed.");
            }

        }
    }
}
