using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PriceTracker.Core.App;
using PriceTrackerTest.Utils.TestAppBuilder.Repository;

namespace PriceTrackerTest.Utils.TestAppBuilder
{

    public class TestAppBuilder : AppBuilder
    {
        public TestAppBuilder()
        {
        }
        public TestApp BuildTestApp(ILogger logger)
        {
            IServiceCollection services = new ServiceCollection();
            UseTestStageRepository(services);

            //UseLogger(services);
            services.AddSingleton<ILogger>(logger);

            UseMerchDataMaintenance(services);
            UseOptions(services);
            var provider = services.BuildServiceProvider();
            TestApp app = new(provider);
            return app;
        }

        private void UseTestStageRepository(IServiceCollection services)
        {
            var constructor = new TestRepositoryConstructor<PriceTrackerTestContext>();
            constructor.AddRepository(services);
        }
    }
}
