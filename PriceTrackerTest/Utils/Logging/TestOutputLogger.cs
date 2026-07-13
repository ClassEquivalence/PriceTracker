using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace PriceTrackerTest.Utils.Logging
{
    // Паттерн адаптер.
    public class TestOutputLogger : ILogger
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly LogLevel _minLevel;
        public TestOutputLogger(ITestOutputHelper outputHelper,
            LogLevel minLogLevel = LogLevel.Trace)
        {
            _outputHelper = outputHelper;
            _minLevel = minLogLevel;
        }
        public IDisposable BeginScope<TState>(TState state) => null!;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= _minLevel ? true : false;

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
