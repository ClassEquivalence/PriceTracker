using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.BaseAppModels;

namespace PriceTracker.Models.DbRelatedModels
{
    public class PriceTrackerContext : DbContext
    {
        public DbSet<Shop> Shops { get; set; }
        public PriceTrackerContext() : base()
        {
            Database.EnsureCreated();
        }
    }
}
