using Microsoft.EntityFrameworkCore;
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
            // TODO: Можно вынести все эти строки в отдельные классы конфигурации.

            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<CitilinkMerchEntity>()
                .HasIndex(m => m.CitilinkId).IsUnique();

            /*
            modelBuilder.Entity<MerchPriceHistoryEntity>()
                .HasMany(h => h.TimestampedPrices)
                .WithOne(p => p.MerchPriceHistory)
                .HasForeignKey(p => p.MerchPriceHistoryId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<MerchPriceHistoryEntity>()
                .HasOne(x => x.CurrentPrice)
                .WithMany()
                .HasForeignKey(x => x.CurrentPriceId).IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            */


        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            //optionsBuilder.EnableSensitiveDataLogging();

            // TODO: Вынести строку подключения в сервис а в сервисе получать из конфига.
            /*
                _logger?.LogTrace("Подключаемся к удаленной БД:" +
                    $"{_conectionString}");
            */
            optionsBuilder.UseNpgsql
                (_conectionString + _connectionPassword);

            optionsBuilder.EnableSensitiveDataLogging(true);

        }
        public PriceTrackerContext(IConfiguration appConfig, ILogger<Program>? logger = null)
        {
            _conectionString = appConfig.GetConnectionString("ProductionDatabase");
            _connectionPassword = appConfig.GetConnectionString("ProductionDatabasePassword");
            _logger = logger;
            //var isCreated = Database.EnsureCreated();
            //logger?.LogDebug($"БД была создана?: {isCreated}");
        }
    }
}
