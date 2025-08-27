using Microsoft.Extensions.Logging;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper;
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
    public class MerchFetchRequestBuilderManualTests
    {
        private readonly MerchFetchRequestBuilder _requestBuilder;

        private readonly ITestOutputHelper _output;
        private readonly CitilinkScraper _scraper;
        private readonly ILogger _testLogger;

        const string _loggingFilePath = "Logging\\Logs.txt";


        public MerchFetchRequestBuilderManualTests(ITestOutputHelper output)
        {
            _output = output;
            var factory = new LoggerFactory();
            var fileLoggerProvider = new FileLoggerProvider(_loggingFilePath);
            var testOutputLoggerProvider = new TestOutputLoggerProvider(output);
            factory.AddProvider(fileLoggerProvider);
            factory.AddProvider(testOutputLoggerProvider);
            var logger = factory.CreateLogger("ManualTestsLogger");
            _testLogger = logger;


            _requestBuilder = new();
        }

        [ManualTheory]
        [InlineData("smartfony", 1)]
        public void RequestHeaders_ManualCheck(string slug, int page)
        {
            var request = _requestBuilder.Build(slug, page);

            foreach(var header in request.Headers)
            {
                foreach(var value in header.Value)
                {
                    _testLogger.LogDebug($"{header.Key}: {value}");
                }
            }

            foreach (var header in request.Content.Headers)
            {
                foreach (var value in header.Value)
                {
                    _testLogger.LogDebug($"{header.Key}: {value}");
                }
            }


            _testLogger.LogDebug($"URI: {request.RequestUri.AbsoluteUri}");

            _testLogger.LogDebug($"{request.Content.ReadAsStringAsync().Result}");
            
        }

        [ManualTheory]
        [InlineData("smartfony", 1)]
        public void ResponseBody_ManualCheck(string slug, int page)
        {
            var request = _requestBuilder.Build(slug, page);

            HttpClient client = new();

            var response = client.SendAsync(request).Result;

            _testLogger.LogDebug($"{response.Content.ReadAsStringAsync().Result}");
        }

    }
}
