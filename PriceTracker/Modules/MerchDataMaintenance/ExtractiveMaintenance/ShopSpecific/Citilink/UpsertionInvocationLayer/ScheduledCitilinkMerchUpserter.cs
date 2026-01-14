using PriceTracker.Core.Configuration.ProvidedWithDI;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.UpsertionInvocationLayer;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer
{
    public class ScheduledCitilinkMerchUpserter :
        ScheduledMerchUpserter<CitilinkMerchParsingDto, CitilinkExtractionStateDto>
    {
        private readonly IAppEnvironment _appEnvironment;


        public ScheduledCitilinkMerchUpserter(CitilinkMerchUpsertionProcessor merchUpsertionService,
            TimeSpan upsertionCyclePeriod,
            IAppEnvironment appEnvironment, DateTime upsertionStartTime, ILogger? logger, 
            TimeSpan upsertionRestPeriod) :
            base(merchUpsertionService, upsertionCyclePeriod, upsertionRestPeriod, upsertionStartTime,
                logger)
        {
            _appEnvironment = appEnvironment;

        }

        public override async Task ProcessUpsertion()
        {
            if (_appEnvironment.IsDevelopment)
            {
                _logger?.LogInformation($"Запущен апсершн Ситилинка.");
                await base.ProcessUpsertion();
            }
            else
            {
                try
                {
                    _logger?.LogInformation($"Запущен апсершн Ситилинка.");
                    await base.ProcessUpsertion();
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"Апсершн ситилинка прекращен из-за исключения: {ex.Message}");
                    throw new InvalidOperationException($"Апсершн ситилинка прекращен из-за исключения: {ex.Message}",
                        ex);
                }
            }
        }

        public override async Task OnShutDownAsync()
        {
            await MerchUpserter.OnProcessHalt();
        }

    }
}
