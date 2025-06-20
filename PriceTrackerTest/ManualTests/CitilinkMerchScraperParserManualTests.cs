
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ScrapingServices.HttpClients.Browser;
using PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine.ShopSpecific.Citilink;
using PriceTrackerTest.Utils.CustomAttributes;
using PriceTrackerTest.Utils.Logging.LoggerProviders;
using Xunit.Abstractions;

namespace PriceTrackerTest.ManualTests
{
    public class CitilinkMerchScraperParserManualTests
    {

        CitilinkMerchParser parser;
        private readonly ITestOutputHelper _output;
        private readonly CitilinkScraper _scraper;
        private readonly ILogger _testLogger;

        const string _loggingFilePath = "Logging\\Logs.txt";

        public CitilinkMerchScraperParserManualTests(ITestOutputHelper output)
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
            var browserAdapter = new BrowserAdapter(browser, (3, 6));
            var scraper = new CitilinkScraper(browserAdapter, logger);
            _scraper = scraper;
            parser = new(scraper, logger);
        }

        [ManualTheory]
        [InlineData("https://www.citilink.ru/catalog/sendvichnicy/?ref=mainmenu_plate")]
        [InlineData("https://www.citilink.ru/catalog/sendvichnicy/?ref=mainmenu_plate&p=2")]
        [InlineData("https://www.citilink.ru/catalog/sendvichnicy/?ref=mainmenu_plate&p=3")]
        public async void ParsePortion_ManualCheck(string url)
        {
            var result = await parser.ParsePortionFromUrl(url);
            
            foreach (var item in result)
            {
                _output.WriteLine($"CitilinkId: {item.CitilinkId}, Price: {item.Price}, Name: {item.Name}");
            }
        }

        [ManualFact]
        public async void RetrieveAllMerchCatalogsUrls_ManualCheck()
        {
            
            var result = parser.RetrieveAllMerchCatalogsUrls();
            
            await foreach(var item in result)
            {
                _output.WriteLine(item);
            }
        }

        [ManualFact]
        public async void RetreiveAllMerches_ManualCheck()
        {
            var asyncEnumerable = parser.RetreiveAll();
            await foreach (var merch in asyncEnumerable)
            {
                _testLogger.LogDebug($"Извлечен товар: Name:{merch.Name}, " +
                    $"Price:{merch.Price}, CitilinkId:{merch.CitilinkId}");
            }
        }

        [ManualTheory]
        [InlineData("https://www.citilink.ru/catalog/sendvichnicy/?ref=mainmenu_plate")]
        public async void ParsePageCount_ManualCheck(string url)
        {
            var node = await _scraper.UrlToNode(url);
            int result = parser.ParsePageCount(node);
            _output.WriteLine($"page count: {result}");
        }


        [ManualTheory]
        [InlineData("https://www.citilink.ru/catalog/sendvichnicy/?ref=mainmenu_plate")]
        public void RetrieveMerchesFromCatalog_ManualCheck(string catalogUrl)
        {
            var result = parser.RetrieveMerchesFromCatalog(catalogUrl).ToBlockingEnumerable();
            foreach(var item in result)
            {
                _output.WriteLine($"CitilinkId: {item.CitilinkId}, Price: {item.Price}, Name: {item.Name}");
            }
        }
    }
}
