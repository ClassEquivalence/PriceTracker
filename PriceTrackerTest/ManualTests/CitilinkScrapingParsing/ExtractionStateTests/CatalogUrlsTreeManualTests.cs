using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
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

namespace PriceTrackerTest.ManualTests.CitilinkScrapingParsing.ExtractionStateTests
{
    public class CatalogUrlsTreeManualTests
    {

        private readonly ITestOutputHelper _output;
        private readonly ILogger _testLogger;

        const string _loggingFilePath = "Logging\\Logs.txt";




        public CatalogUrlsTreeManualTests(ITestOutputHelper output)
        {

            _output = output;
            var factory = new LoggerFactory();
            var fileLoggerProvider = new FileLoggerProvider(_loggingFilePath);
            var testOutputLoggerProvider = new TestOutputLoggerProvider(output);
            factory.AddProvider(fileLoggerProvider);
            factory.AddProvider(testOutputLoggerProvider);
            var logger = factory.CreateLogger("ManualTestsLogger");
            _testLogger = logger;



        }

        [ManualFact]
        public void RemoveFiltersAndDuplicates_ManualCheck()
        {
            BranchWithFunctionality ch1 = new(default, "https://www.citilink.ru/catalog/platformy-dlya-sborki-pk" +
                "--platformy-dlya-sborki-pk-mainmenu/?ref=mainmenu_left", []);

            BranchWithFunctionality ch2 = new(default, "https://www.citilink.ru/catalog/platformy-dlya-sborki-pk" +
                "/?ref=mainmenu_left", []);

            BranchWithFunctionality root = new(default, "https://www.citilink.ru/catalog/", [ch1, ch2]);

            CitilinkCatalogUrlsTree tree = new(root);

            tree.RemoveBranchFiltersAndDuplicates();

            foreach(var branch in tree.GetAllBranches())
            {
                _testLogger.LogInformation($"{branch}");
            }
        }
    }
}
