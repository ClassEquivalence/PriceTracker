using Microsoft.Extensions.Logging;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Pipeline;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;
using PriceTracker.Modules.Repository;
using PriceTrackerTest.Utils.Logging;
using PriceTrackerTest.Utils.TestAppBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _testApp = appBuilder.BuildWithDefaults(new TestOutputLogger(outputHelper));

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
