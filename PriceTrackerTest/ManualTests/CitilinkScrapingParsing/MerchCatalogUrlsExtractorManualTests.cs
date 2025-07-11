using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink.Engine;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Utils.ScrapingServices.HttpClients.Browser;
using PriceTrackerTest.Utils.CustomAttributes;
using PriceTrackerTest.Utils.Logging.LoggerProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace PriceTrackerTest.ManualTests.CitilinkScrapingParsing
{
    public class MerchCatalogUrlsExtractorManualTests
    {


        CitilinkMerchParser parser;
        MerchCatalogUrlsExtractor urlsExtractor;
        private readonly ITestOutputHelper _output;
        private readonly CitilinkScraper _scraper;
        private readonly ILogger _testLogger;

        const string _loggingFilePath = "Logging\\Logs.txt";

        public MerchCatalogUrlsExtractorManualTests(ITestOutputHelper output)
        {
            _output = output;
            var factory = new LoggerFactory();
            var fileLoggerProvider = new FileLoggerProvider(_loggingFilePath);
            var testOutputLoggerProvider = new TestOutputLoggerProvider(output);
            factory.AddProvider(fileLoggerProvider);
            factory.AddProvider(testOutputLoggerProvider);
            var logger = factory.CreateLogger("ManualTestsLogger");
            _testLogger = logger;

            var playwrightTask = Playwright.CreateAsync();
            var browser = playwrightTask.Result.Chromium.LaunchAsync().Result;

            var browserAdapter = new BrowserAdapter(browser, (15, 30));
            var scraper = new CitilinkScraper(browserAdapter, logger);
            _scraper = scraper;
            parser = new(scraper, logger);

            urlsExtractor = new(scraper, logger);

        }


        [ManualFact]
        public async void RetrieveAllMerchCatalogsUrls_ManualCheck()
        {

            var result = urlsExtractor.RetrieveAllMerchCatalogsUrls();

            await foreach (var item in result)
            {
                _output.WriteLine(item);
            }
        }

        [ManualFact]
        public async void ParseSubCatalogsUrlsFromMain_ManualCheck()
        {
            var mainCatalogSectionsNode = await _scraper.UrlToNode("https://www.citilink.ru/catalog/");
            var result = urlsExtractor.ParseSubCatalogsUrlsFromMain(mainCatalogSectionsNode);

            foreach (var item in result)
            {
                _output.WriteLine(item);
            }
        }


    }
}
