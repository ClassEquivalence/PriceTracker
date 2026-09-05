using Microsoft.Extensions.Logging;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;
using PriceTrackerTest.Utils.Logging;
using PriceTrackerTest.Utils.TestAppBuilder;
using Xunit.Abstractions;

namespace PriceTrackerTest.IntegrationTests
{
    public class CitilinkUpsertionInvokerIntegrationTests
    {
        private readonly TestApp _testApp;
        private readonly ICitilinkUpsertionInvoker _upsertionInvoker;
        private readonly ILogger _logger;
        public CitilinkUpsertionInvokerIntegrationTests(ITestOutputHelper outputHelper)
        {
            TestAppBuilder appBuilder = new();
            _testApp = appBuilder.BuildTestApp(new TestOutputLogger(outputHelper));

            _testApp.Initialize();

            _upsertionInvoker = _testApp.ProvideUpsertionInvoker();
            _logger = _testApp.ProvideLogger();
        }

        [Fact]
        public async Task PartialCycleBehaviorOverview()
        {
            CitilinkEnforcersSharedContext sharedContext = new(5, 200);
            var result = await _upsertionInvoker.ExecutePartialCycleAsync(sharedContext,
                PartialCycleExecutionMode.StartAnew);
            _logger?.LogDebug($"{result}");
        }
    }
}
