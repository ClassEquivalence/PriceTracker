using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Entities.Domain.MerchPriceHistory;
using PriceTracker.Modules.Repository.Entities.Domain.ShopSpecific;
using PriceTracker.Modules.Repository.Entities.Infrastructure;
using PriceTracker.Modules.Repository.Entities.Process;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction;

namespace PriceTracker.Modules.Repository.DataAccess.EFCore
{
    public class PriceTrackerContext : DbContext
    {

        private readonly string? _conectionString;
        private readonly string? _connectionPassword;
        private readonly ILogger? _logger;

        public DbSet<ShopEntity> Shops { get; set; }
        public DbSet<MerchEntity> Merches { get; set; }
        public DbSet<CitilinkMerchEntity> CitilinkMerches { get; set; }
        public DbSet<TimestampedPriceEntity> TimestampedPrices { get; set; }
        public DbSet<MerchPriceHistoryEntity> MerchPriceHistoryEntities { get; set; }

        public DbSet<TimeExtractionProcessHappened> TimeExtractionProcessHappened { get; set; }
        public DbSet<CitilinkParsingExecutionStateEntity> CitilinkParsingExecutionStateEntity
        { get; set; }
        public DbSet<CitilinkExtractorStorageStateEntity> CitilinkExtractionStorageStates { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<CitilinkMerchEntity>()
                .HasIndex(m => m.CitilinkId).IsUnique();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql
                (_conectionString + _connectionPassword + "Include Error Detail=true;");

            optionsBuilder.EnableSensitiveDataLogging(true);

        }
        public PriceTrackerContext(IOptions<ProductionDbOptions> options, ILogger<Program>? logger = null)
        {
            _conectionString = options.Value.ConnectionStringBody;
            _connectionPassword = options.Value.ConnectionPassword;
            _logger = logger;

            if (string.IsNullOrEmpty(_conectionString))
            {
                throw new ArgumentException($"{nameof(PriceTrackerContext)}: ConnectionString is null");
            }
            else
            {
                _logger?.LogTrace($"{nameof(PriceTrackerContext)}: Попытка подключиться к БД");
            }

        }
    }
}
