using PriceTracker.Core.Configuration.ProvidedWithDI;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Upsertion;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner.Parsers;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;

namespace PriceTracker.Core.App
{
    public class AppBuilder
    {
        public AppBuilder()
        {

        }

        public WebApplication BuildDevelopmentStageApp()
        {
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;
            UseProductionRepository(services);
            UseLogger(services);
            UseMerchDataMaintenance(services);
            UseOptions(services);
            UseAngular(services);

            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("AllowAngularDev");
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            return app;
        }
        protected void UseProductionRepository(IServiceCollection collection)
        {
            RepositoryConstructor constructor = new();
            constructor.AddRepository(collection);
        }
        protected void UseLogger(IServiceCollection collection)
        {
            collection.AddLogging();
            collection.AddSingleton<ILogger>(s => s.GetService<ILogger<Program>>());
        }
        protected void UseMerchDataMaintenance(IServiceCollection collection)
        {
            collection.AddSingleton<IInnerCitilinkMerchUpserter, InnerCitilinkMerchUpserter>();
            collection.AddSingleton<ICitilinkParsableClassifier, CitilinkParsablesClassifier>();
            collection.AddSingleton<ICitilinkCatalogsCatalogParser, CitilinkCatalogsCatalogParser>();
            collection.AddSingleton<ICitilinkMerchCatalogParser, CitilinkMerchCatalogParser>();
            collection.AddSingleton<ICitilinkRootCatalogParser, CitilinkRootCatalogParser>();

            collection.AddSingleton<HttpClient>();

            collection.AddSingleton<IInnerCitilinkParser, InnerCitilinkParser>();

            collection.AddSingleton<CitilinkInnerDataAugmenter>();

            collection.AddSingleton<CitilinkEnforcersSharedContext>();

            collection.AddSingleton<ICitilinkScraper,
                CitilinkScraper>();
            collection.AddSingleton<CitilinkPartialCycleExecutor>();
        }
        protected void UseOptions(IServiceCollection collection)
        {
            ConfigurationManager configurationManager = new();
            configurationManager.AddJsonFile("appsettings.json");
            collection.Configure<ProductionDbOptions>(
                configurationManager.GetSection(ProductionDbOptions.OptionsSectionKey));
            collection.Configure<MerchUpsertionOptions>(
                configurationManager.GetSection(MerchUpsertionOptions.OptionsSectionKey));
            collection.Configure<CitilinkUpsertionOptions>(
                configurationManager.GetSection(CitilinkUpsertionOptions.OptionsSectionKey));
            collection.AddSingleton<IAppEnvironment, HostEnvironmentAdapter>();
        }
        protected void UseAngular(IServiceCollection collection)
        {
            collection.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDev", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
        }
    }
}
