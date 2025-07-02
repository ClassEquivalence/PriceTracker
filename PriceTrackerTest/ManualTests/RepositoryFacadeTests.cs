using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using PriceTracker.Core.Models.Domain;
using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Facade;
using PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink;
using PriceTrackerTest.Utils.CustomAttributes;
using PriceTrackerTest.Utils.Logging;
using PriceTrackerTest.Utils.Logging.LoggerProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace PriceTrackerTest.ManualTests
{
    public class RepositoryFacadeTests
    {
        private readonly IRepositoryFacade _repository;

        const string _loggingFilePath = "Logging\\Logs.txt";
        private readonly ILogger _testLogger;

        public RepositoryFacadeTests(ITestOutputHelper output)
        {
            var factory = new LoggerFactory();
            var fileLoggerProvider = new FileLoggerProvider(_loggingFilePath);
            var testOutputLoggerProvider = new TestOutputLoggerProvider(output);
            factory.AddProvider(fileLoggerProvider);
            factory.AddProvider(testOutputLoggerProvider);
            var logger = factory.CreateLogger("ManualTestsLogger");
            _testLogger = logger;


            PriceTrackerContext dbContext = new();
            _repository = new RepositoryFacade(dbContext, _testLogger);

        }

        [Fact]
        public void TestInsertion()
        {
            ICitilinkMerchRepositoryFacade citilinkMerchRepository =
                _repository;

            TimestampedPriceDto currentPrice = new(default, 1000, DateTime.Now, default);
            List<TimestampedPriceDto> timestampedPrices = [];
            MerchPriceHistoryDto priceTrack = new(default, timestampedPrices, currentPrice,
                default);
            ShopDto citilink = _repository.GetCitilinkShop();
            CitilinkMerchDto merch = new(default, "Lolk", priceTrack, "19582156",
                citilink.Id, default);
            citilinkMerchRepository.TryInsert(merch);
        }
    }
}
