using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace PriceTrackerTest.Utils.Logging.LoggerProviders
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _filePath;
        private readonly ConcurrentDictionary<string, FileLogger> _loggers = new();
        public FileLoggerProvider(string filePath)
        {
            _filePath = filePath;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new FileLogger(_filePath));
        }

        public void Dispose() { }
    }
}
