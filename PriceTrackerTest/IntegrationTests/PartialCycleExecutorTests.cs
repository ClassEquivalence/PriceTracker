using Microsoft.Extensions.Logging;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;
using PriceTrackerTest.Utils.Logging;
using PriceTrackerTest.Utils.TestAppBuilder;
using Xunit.Abstractions;

namespace PriceTrackerTest.IntegrationTests
{
    public class PartialCycleExecutorTests
    {
        private readonly TestApp _testApp;
        private readonly CitilinkPartialCycleExecutor _executor;
        private readonly ILogger _logger;
        public PartialCycleExecutorTests(ITestOutputHelper outputHelper)
        {
            TestAppBuilder appBuilder = new();
            _testApp = appBuilder.BuildTestApp(new TestOutputLogger(outputHelper));

            _testApp.Initialize();

            _executor = _testApp.ProvideCycleExecutor();
            _logger = _testApp.ProvideLogger();
        }

        [Fact]
        public async Task Executor_Executes()
        {
            var partialCycleResult = await _executor.ExecutePartialCycleAsync(5);
            _logger?.LogInformation($"Cycle result is {partialCycleResult}");
        }
    }
}
