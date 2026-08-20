using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Entities.Domain.MerchPriceHistory;
using PriceTracker.Modules.Repository.Entities.Domain.ShopSpecific;
using PriceTracker.Modules.WebInterface.Adapters.Repository;
using PriceTrackerTest.Utils.Logging;
using PriceTrackerTest.Utils.TestAppBuilder;
using Xunit.Abstractions;

namespace PriceTrackerTest.IntegrationTests.RepositoryIntegrationTests
{
    public class MerchRepositoryAdapterTests
    {
        private TestApp _testApp;
        private IMerchRepositoryAdapter _merchRepository;

        ITestOutputHelper _testOutputHelper;

        private const int merchId = 1423;
        private const string citilinkMerchCode = "someAbsolutelyUniqueId";
        public MerchRepositoryAdapterTests(ITestOutputHelper outputHelper)
        {
            _testOutputHelper = outputHelper;
        }
        private void ArrangeRepositoryData()
        {
            TestAppBuilder appBuilder = new();
            _testApp = appBuilder.BuildWithDefaults(new TestOutputLogger(_testOutputHelper));
            _testApp.Initialize();
            _merchRepository = _testApp.ProvideMerchRepositoryAdapter();

            using var context = _testApp.ProvideDbContextFactory().CreateDbContext();
            MerchPriceHistoryEntity priceHistoryEntity = new();
            TimestampedPriceEntity currentPriceEntity = new(46810, DateTime.Now);
            priceHistoryEntity.TimestampedPrices = [new(1111, DateTime.Now),
            new(2345, DateTime.Now), currentPriceEntity];
            priceHistoryEntity.CurrentPricePointer = new CurrentPricePointerEntity(default, default,
                default);
            priceHistoryEntity.CurrentPricePointer.CurrentPrice = currentPriceEntity;
            //priceHistoryEntity.CurrentPricePointer.MerchPriceHistory = priceHistoryEntity;
            CitilinkMerchEntity merchEntity = new("SomeVeryMemorialName", citilinkMerchCode, merchId);
            merchEntity.PriceHistory = priceHistoryEntity;
            merchEntity.ShopId = context.Shops.Single(s => s.Name == "Citilink").Id;
            context.Merches.Add(merchEntity);

            context.SaveChanges();
        }

        [Fact]
        public async Task CheckDataIntegrity_SelectingByMerchId()
        {
            ArrangeRepositoryData();

            var merch = await _merchRepository.GetMerchAsync(merchId);
            Assert.NotNull(merch);
            var priceHistory = merch.MerchPriceHistory;
            Assert.NotNull(priceHistory);
            var currentPrice = priceHistory.CurrentPrice;
            Assert.NotNull(currentPrice);
            var count = priceHistory.PreviousTimestampedPricesList.Count;
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task CheckDataIntegrity_SelectingByCitilinkCode()
        {
            ArrangeRepositoryData();

            var merch = await _merchRepository.GetCitilinkMerchAsync(citilinkMerchCode);
            Assert.NotNull(merch);
            var priceHistory = merch.MerchPriceHistory;
            Assert.NotNull(priceHistory);
            var currentPrice = priceHistory.CurrentPrice;
            Assert.NotNull(currentPrice);
            var count = priceHistory.PreviousTimestampedPricesList.Count;
            Assert.Equal(2, count);
        }
    }
}
