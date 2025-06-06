using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;

namespace PriceTracker.Modules.Repository.DataAccess.EFCore
{
    public class PriceTrackerContext : DbContext
    {
        public DbSet<ShopEntity> Shops { get; set; }
        public DbSet<MerchEntity> Merches { get; set; }
        public DbSet<TimestampedPriceEntity> TimestampedPrices { get; set; }
        public DbSet<MerchPriceHistoryEntity> MerchPriceHistoryEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO: Можно вынести все эти строки в отдельные классы конфигурации.

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MerchPriceHistoryEntity>()
                .HasMany(h => h.TimestampedPrices)
                .WithOne(p => p.MerchPriceHistory)
                .HasForeignKey(p => p.MerchPriceHistoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MerchPriceHistoryEntity>()
                .HasOne(h => h.CurrentPrice)
                .WithMany() // нет навигационного свойства в TimestampedPriceEntity
                .HasForeignKey(h => h.CurrentPriceId)
                .OnDelete(DeleteBehavior.Restrict); // чтобы EF не пытался удалить CurrentPrice при cascade


            modelBuilder.Entity<MerchEntity>()
                .HasOne(m => m.PriceHistory)
                .WithOne(ph => ph.Merch)
                .HasForeignKey<MerchPriceHistoryEntity>(ph => ph.MerchId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: Вынести строку подключения в сервис а в сервисе получать из конфига.
            optionsBuilder.UseNpgsql
                ("Server=localhost; Database=PriceTracker; User ID=postgres; Password=123; Port=5432;");
        }
        public PriceTrackerContext(ILogger<Program> logger)
        {
            var isCreated = Database.EnsureCreated();
            logger.LogDebug($"БД была создана?: {isCreated}");
        }
    }
}
