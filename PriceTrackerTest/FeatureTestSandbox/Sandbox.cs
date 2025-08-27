using Microsoft.Extensions.Logging;
using PriceTrackerTest.Utils.CustomAttributes;
using PriceTrackerTest.Utils.Logging.LoggerProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace PriceTrackerTest.FeatureTestSandbox
{
    public class Sandbox
    {

        private readonly ITestOutputHelper _output;
        private readonly ILogger _testLogger;

        const string _loggingFilePath = "Logging\\Logs.txt";

        public Sandbox(ITestOutputHelper output)
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
        public void CheckJsonDeserialization()
        {
            string jsonstr = @"{""lol"": ""dsgsd"", ""muh"": ""ags""}";

            _testLogger.LogInformation(jsonstr);

            var obj = JsonSerializer.Deserialize<SomeJsonObject>(jsonstr);
            _testLogger.LogInformation($"{obj.muh}, {obj.lol}");
        }


}
}
