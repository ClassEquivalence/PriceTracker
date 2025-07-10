using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace PriceTrackerTest.Utils.Logging
{
    // Паттерн адаптер.
    public class TestOutputLogger : ILogger
    {
        private readonly ITestOutputHelper _outputHelper;
        public TestOutputLogger(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }
        public IDisposable BeginScope<TState>(TState state) => null!;

        public bool IsEnabled(LogLevel logLevel) => logLevel > LogLevel.Trace ? true : false;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            _outputHelper.WriteLine($"[{logLevel}] {formatter(state, exception)}\n");
            if (exception != null)
            {
                _outputHelper.WriteLine(exception.ToString());
            }
        }
    }
}
