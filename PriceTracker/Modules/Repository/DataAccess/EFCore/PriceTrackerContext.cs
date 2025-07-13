using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink;
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
            optionsBuilder.UseNpgsql
                ("Server=localhost; Database=PriceTracker; User ID=postgres; Password=123; Port=5432;");
        }
        public PriceTrackerContext(ILogger<Program>? logger = null)
        {
            //var isCreated = Database.EnsureCreated();
            //logger?.LogDebug($"БД была создана?: {isCreated}");
        }
    }
}
