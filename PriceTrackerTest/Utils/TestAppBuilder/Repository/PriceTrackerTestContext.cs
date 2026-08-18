using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PriceTracker.Modules.Repository.DataAccess.EFCore;

namespace PriceTrackerTest.Utils.TestAppBuilder.Repository
{
    public class PriceTrackerTestContext : PriceTrackerContext
    {
        private SqliteConnection _connection;
        public PriceTrackerTestContext(SqliteConnection connection,
            ILogger? logger) : base(logger)
        {
            _connection = connection;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var options = optionsBuilder
                .UseSqlite(_connection) // Передаем именно открытое соединение, а не просто строку!
                                        // Вроде как если передавать строку - возникнут проблемы связанные со статусом InMemory.
                .LogTo(s => _logger?.LogTrace(s)); // Вывод SQL в консоль для тестирования
        }

    }
}
