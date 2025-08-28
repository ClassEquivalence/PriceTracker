using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper;
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
    /*
    public class MerchCatalogUrlsParserManualTests
    {

        CitilinkMerchCatalogUrlsParser urlsParser;


        private readonly ITestOutputHelper _output;
        private readonly CitilinkScraper _scraper;
        private readonly ILogger _testLogger;

        const string _loggingFilePath = "Logging\\Logs.txt";

        public MerchCatalogUrlsParserManualTests(ITestOutputHelper output)
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

            BranchWithHtml root = new(default, Configs.CitilinkMainCatalogUrl, []);

            CitilinkCatalogUrlsTree tree = new(root);

            urlsParser = new(_scraper, tree, _testLogger);

            
        }

        [ManualFact]
        public async void GetMerchCatalogUrlsPortion_ManualCheck()
        {

            await _scraper.PerformInitialRunupAsync();

            var branches = await urlsParser.GetMerchCatalogUrlsPortion();

            StringBuilder logOutput = new("Url-адреса каталогов товаров:");

            if (branches == null || !branches.Any())
                throw new Exception("Ветки не должны отсутствовать.");



            foreach(var branch in branches)
            {
                logOutput = logOutput.AppendLine($"Url-адрес ветви: {branch.Url}");
            }

            _testLogger.LogDebug(logOutput.ToString());

        }


    }
    */
}
