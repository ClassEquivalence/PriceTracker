using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.Facade;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Infrastructure
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

            services.AddSingleton<CitilinkMerchDataUpserter>();
            services.AddSingleton<GUICitilinkExtractor>();

            /*
            ExtractionStateSavePolicy
            extractionStateSavePolicy = ExtractionStateSavePolicy.OnUpsertionHaltIssued
             */

            services.AddSingleton<CitilinkMerchUpsertionProcessor>();
            /*
            TimeSpan upsertionCyclePeriod,
            IAppEnvironment appEnvironment, DateTime upsertionStartTime, ILogger? logger, 
            TimeSpan upsertionRestPeriod
             */
            services.AddSingleton<ScheduledMerchUpserter, ScheduledCitilinkMerchUpserter>();
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
