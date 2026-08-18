using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.CoordinationLayer;
using PriceTracker.Modules.MerchDataMaintenance.Facade;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Infrastructure
{
    public static class MerchUpserterDependencyInjection
    {

        /*
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
                */




        private static IServiceCollection UseCitilinkMerchUpserter
            (this IServiceCollection services) //incompleted!
        {




            /*
             *upsertionCyclePeriod! upsertionStartTime!
             *upsertionRestPeriod! extractionStateSavePolicy!
             *
             *public ScheduledCitilinkMerchUpserter(CitilinkMerchDataUpserter
            dataConsumer, GUICitilinkExtractor dataExtractor, TimeSpan upsertionCyclePeriod,
            IAppEnvironment appEnvironment, DateTime upsertionStartTime, 
            ICitilinkMiscellaneousRepositoryFacade repository, ILogger? logger, TimeSpan
            upsertionRestPeriod, ExtractionStateSavePolicy
            extractionStateSavePolicy = ExtractionStateSavePolicy.OnServerShutdown)
             * 
            */


            /*
            ExtractionStateSavePolicy
            extractionStateSavePolicy = ExtractionStateSavePolicy.OnUpsertionHaltIssued
             */

            /*
            TimeSpan upsertionCyclePeriod,
            IAppEnvironment appEnvironment, DateTime upsertionStartTime, ILogger? logger, 
            TimeSpan upsertionRestPeriod
             */
            return services;
        }

        public static IServiceCollection AddMerchUpserter(this IServiceCollection services)
        {

            services = services.UseCitilinkMerchUpserter();

            services.AddSingleton<UpsertionService>();

            //top level
            services.AddSingleton<IMerchDataProviderFacade, MerchDataProviderFacade>();
            services.AddSingleton<IHostedService>(sp => sp.
            GetRequiredService<IMerchDataProviderFacade>());
            return services;
        }

    }
}
