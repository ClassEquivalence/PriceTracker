using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.EntityLevel
{
    public class TimestampedPriceEntityRepository : EFGenericEntityRepository<TimestampedPriceEntity>
    {
        public TimestampedPriceEntityRepository(DbContext dbContext) : base(dbContext)
        {
        }

        protected override IQueryable<TimestampedPriceEntity> entitiesWithIncludes =>
            entities.Include(tp => tp.MerchPriceHistory).ThenInclude(ph => ph.Merch)
            .ThenInclude(m => m.Shop).ThenInclude(s => s.Merches).
            ThenInclude(m => m.PriceHistory).ThenInclude(ph => ph.TimestampedPrices);
    }
}
