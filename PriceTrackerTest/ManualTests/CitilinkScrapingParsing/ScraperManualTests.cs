using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink.Engine;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Utils.ScrapingServices.HttpClients.Browser;
using PriceTrackerTest.Utils.CustomAttributes;
using PriceTrackerTest.Utils.Logging.LoggerProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit.Abstractions;

namespace PriceTrackerTest.ManualTests.CitilinkScrapingParsing
{
    public class ScraperManualTests
    {
        private readonly ITestOutputHelper _output;
        private readonly CitilinkScraper _scraper;
        private readonly ILogger _testLogger;
        private readonly IBrowser _browser;
        private readonly BrowserAdapter _browserAdapter;

        const string _loggingFilePath = "Logging\\Logs.txt";

        public ScraperManualTests(ITestOutputHelper output)
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
            _browser = browser;

            var browserAdapter = new BrowserAdapter(browser, (15, 30));
            _browserAdapter = browserAdapter;
            var scraper = new CitilinkScraper(browserAdapter, logger);
            _scraper = scraper;

        }

        [ManualFact]
        public async void UrlToNode_ManualCheck()
        {
            var node = await _scraper.UrlToNodeAsync("https://www.citilink.ru/catalog/");
            _testLogger.LogDebug($"{node.GetDirectInnerText()}");
        }

        [ManualFact]
        public async void Browser_UrlToNode_ManualCheck()
        {
            var page = await _browser.NewPageAsync();
            var response = await page.GotoAsync("https://www.citilink.ru/catalog/");
            await Task.Delay(10000);

            response = await page.GotoAsync("https://www.citilink.ru/catalog/");


            _testLogger.LogDebug($"Response text: {response?.StatusText}\n" +
                $"Response statuscode: {response?.Status}");

            StringBuilder sb = new StringBuilder();
            var respHeaders = await response.AllHeadersAsync();
            foreach (var header in respHeaders)
            {
                sb.Append($"{header.Key}: {header.Value}\n");
            }
            _testLogger.LogDebug($"Response headers: {sb}\n\n");

            StringBuilder sb2 = new StringBuilder();
            var reqHeaders = await response.Request.AllHeadersAsync();
            foreach (var header in reqHeaders)
            {
                sb2.Append($"{header.Key}: {header.Value}\n");
            }
            _testLogger.LogDebug($"Request headers: {sb2}\n\n");


            
            var content = await page.ContentAsync();

            _testLogger.LogDebug($"HtmlContent: {content}");
        }

        [ManualFact]
        public async void BrowserAdapter_UrlToNode_ManualCheck()
        {
            var res = await _browserAdapter.UrlToHtmlAsync("https://www.citilink.ru/catalog/",
                msDelayBetweenAttempts: 20000);
            _testLogger.LogDebug($"HtmlContent: {res}");
        }

    }
}
