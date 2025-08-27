using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.MerchParser;
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
    public class MerchParserManualTests
    {
        private readonly CitilinkMerchParser _merchParser;

        private readonly ITestOutputHelper _output;
        private readonly CitilinkScraper _scraper;
        private readonly ILogger _testLogger;

        const string _loggingFilePath = "Logging\\Logs.txt";




        public MerchParserManualTests(ITestOutputHelper output)
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



            _merchParser = new(_scraper, _testLogger);
        }

        [ManualTheory]
        [InlineData(@"https://www.citilink.ru/catalog/noutbuki/")]
        public async void RetreiveAllFromMerchCatalog_ManualCheck(string merchCatalogUrl)
        {
            await _scraper.PerformInitialRunupAsync();

            BranchWithHtml merchCatalogBranch = new(default, merchCatalogUrl, []);
            var merches = _merchParser.RetreiveAllFromMerchCatalog(merchCatalogBranch).
                ToBlockingEnumerable();
            var list = merches.ToList();

            _testLogger.LogDebug($"merches count: {list.Count()}\n" +
                $"first: {list[0]}\n" +
                $"last: {list[^1]}");

        }

    }
}
