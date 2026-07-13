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
        public PriceTrackerTestContext(ILogger<Program>? logger = null) : base(logger)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("SomeDBName");
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
