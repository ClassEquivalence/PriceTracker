using PriceTracker.Core.Models.Domain;
using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.WebInterface.Adapters.Repository;
using PriceTrackerTest.Utils.Logging;
using PriceTrackerTest.Utils.TestAppBuilder;
using Xunit.Abstractions;

namespace PriceTrackerTest.IntegrationTests.RepositoryIntegrationTests
{
    public class MerchRepositoryAdapterTests
    {
        private readonly TestApp _testApp;
        private readonly IMerchRepositoryAdapter _merchRepository;

        private const int merchId = 1423;
        private const string citilinkMerchCode = "someAbsolutelyUniqueId";
        public MerchRepositoryAdapterTests(ITestOutputHelper outputHelper)
        {
            // Arrange Act Assert.
            // Arrange-part
            TestAppBuilder appBuilder = new();
            _testApp = appBuilder.BuildWithDefaults(new TestOutputLogger(outputHelper));
            _testApp.Initialize();
            _merchRepository = _testApp.ProvideMerchRepositoryAdapter();
            ArrangeRepositoryData();
            // Arrange-part ends here.
        }
        private void ArrangeRepositoryData()
        {
            // используем для этого временно старый пласт кода.
            var oldMerchRepository = _testApp.ProvideMerchRepository();

            // Костыль: чтобы добавить мерч к именно ситилинку - пока что приходится узнавать его ID
            // при обращении к репозиторию.
            var citilink = _testApp.ProvideShopRepository().SingleOrDefault(s => s.Name == "Citilink");
            if (citilink == null)
                throw new InvalidOperationException();

            MerchPriceHistoryDto priceHistory = new(default, [new(default, 1111, DateTime.Now, default),
            new(default, 2345, DateTime.Now, default)], new(default, 46810, DateTime.Now, default), default);

            CitilinkMerchDto dto = new(merchId, "SomeVeryMemorialName", priceHistory,
                citilinkMerchCode, citilink.Id, default);
            oldMerchRepository.TryInsert(dto);
        }

        [Fact]
        public async Task CheckDataIntegrity_SelectingByMerchId()
        {
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
