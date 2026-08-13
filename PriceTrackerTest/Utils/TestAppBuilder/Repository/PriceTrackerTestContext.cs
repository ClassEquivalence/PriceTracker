using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PriceTracker;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
