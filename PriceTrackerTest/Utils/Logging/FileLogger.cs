using Microsoft.Extensions.Logging;

namespace PriceTrackerTest.Utils.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath;
        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }
        public IDisposable BeginScope<TState>(TState state) => null!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            File.AppendAllText(_filePath, $"[{logLevel}][{DateTime.Now}] {formatter(state, exception)}\n");

            if (exception != null)
            {
                File.AppendAllText(_filePath, exception.ToString());
            }
        }
    }
}
