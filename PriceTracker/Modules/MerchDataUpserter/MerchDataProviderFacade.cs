using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PriceTracker.Core.Configuration.ProvidedWithDI;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using PriceTracker.Modules.Repository.Repositories.Base;


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
        private UpsertionService _scheduledUpserter;
        private readonly ILogger _logger;
        private readonly MerchUpsertionOptions _options;
        private readonly IAppEnvironment _appEnvironment;


        private readonly ICitilinkMerchRepositoryFacade _citilinkMerchRepository;
        private readonly IShopSelectorFacade _shopSelector;
        private readonly ICitilinkMiscellaneousRepositoryFacade _citilinkMiscellaneousRepository;

        public MerchDataProviderFacade(ILogger<Program> logger,
            IOptions<MerchUpsertionOptions> options, IAppEnvironment appEnvironment, 
            ICitilinkMerchRepositoryFacade citilinkMerchRepository, IShopSelectorFacade
            shopSelector, ICitilinkMiscellaneousRepositoryFacade citilinkMiscellaneousRepository)
        {
            _options = options.Value;
            _appEnvironment = appEnvironment;
            
            _logger = logger;

            _citilinkMerchRepository = citilinkMerchRepository;
            _shopSelector = shopSelector;
            _citilinkMiscellaneousRepository = citilinkMiscellaneousRepository;
            


            

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
                var citilinkOptions = _options.CitilinkUpsertionOptions;
                CitilinkMerchDataUpserter consumer = new(_citilinkMerchRepository, 
                    _shopSelector.GetCitilinkShop(), logger: _logger);
                var CitilinkStorageState = _citilinkMiscellaneousRepository.
                GetExtractorStorageState();
                GUICitilinkExtractor extractor = new(new(), citilinkOptions, _options.UserAgent, _logger);

                ScheduledCitilinkMerchUpserter
                    scheduledCitilinkMerchUpserter = new(consumer, extractor, TimeSpan.
                    FromDays(citilinkOptions.CitilinkPriceUpdatePeriod), _appEnvironment,
                    DateTime.Now, _citilinkMiscellaneousRepository, _logger, 
                    TimeSpan.FromHours(citilinkOptions.MinCooldownForPageRequests));

                _scheduledUpserter = new([scheduledCitilinkMerchUpserter]);


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
