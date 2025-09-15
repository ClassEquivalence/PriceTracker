using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PriceTracker.Core.Configuration.ProvidedWithDI;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;


/*
 Сделать передачу в репозиторий состояния выполнения
 Да и вообще зарефакторить весь модуль надо бы.
 */
namespace PriceTracker.Modules.MerchDataProvider
{
    /// <summary>
    /// Точка взаимодействия с модулем, и модуля - с внешним окружением.
    /// </summary>
    public class MerchDataProviderFacade : IMerchDataProviderFacade
    {
        private readonly UpsertionService _scheduledUpserter;
        private readonly IRepositoryFacade _repository;
        private readonly ILogger _logger;
        private readonly MerchUpsertionOptions _options;
        private readonly IAppEnvironment _appEnvironment;

        public MerchDataProviderFacade(IRepositoryFacade repository, ILogger<Program> logger,
            IOptions<MerchUpsertionOptions> options, IAppEnvironment appEnvironment)
        {
            _options = options.Value;
            _appEnvironment = appEnvironment;
            var citilinkOptions = _options.CitilinkUpsertionOptions;
            _logger = logger;

            IExtractionExecutionStateProvider<CitilinkExtractionStateDto> executionStateProvider
                = repository;
            _repository = repository;


            CitilinkMerchDataUpserter consumer = new(repository, repository.GetCitilinkShop(),
                logger: _logger);


            var CitilinkStorageState = ((ICitilinkMiscellaneousRepositoryFacade)_repository).
                GetExtractorStorageState();
            GUICitilinkExtractor extractor = new(new(), citilinkOptions, _options.UserAgent, _logger);

            ScheduledCitilinkMerchUpserter
                scheduledCitilinkMerchUpserter = new(consumer, extractor, TimeSpan.
                FromDays(citilinkOptions.CitilinkPriceUpdatePeriod), _appEnvironment,
                DateTime.Now, repository, _logger, TimeSpan.FromHours(citilinkOptions.MinCooldownForPageRequests));

            _scheduledUpserter = new([scheduledCitilinkMerchUpserter]);

        }

        public async Task ProcessMerchUpsertion()
        {
            _logger.LogInformation($"{nameof(MerchDataProviderFacade)}: " +
                $"Запущен процесс upsert'а товаров.");

            if (_appEnvironment.IsDevelopment)
            {
                await _scheduledUpserter.ProcessUpsertion();
            }
            else
            {
                try
                {
                    await _scheduledUpserter.ProcessUpsertion();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.InnerException?.Message);
                }
            }

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
