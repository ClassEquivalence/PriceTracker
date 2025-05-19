using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace PriceTrackerTest.Utils.Logging.LoggerProviders
{
    public class TestOutputLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ConcurrentDictionary<string, TestOutputLogger> _loggers = new();
        public TestOutputLoggerProvider(ITestOutputHelper helper)
        {
            _testOutputHelper = helper;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new TestOutputLogger(_testOutputHelper));
        }

        public void Dispose() { }
    }
}
