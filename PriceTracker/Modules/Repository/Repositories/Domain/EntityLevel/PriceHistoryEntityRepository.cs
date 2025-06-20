using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.EntityLevel
{
    public class PriceHistoryEntityRepository : EFGenericEntityRepository
        <MerchPriceHistoryEntity>
    {
        public PriceHistoryEntityRepository(DbContext dbContext) : base(dbContext)
        {
        }

        protected override IQueryable<MerchPriceHistoryEntity> entitiesWithIncludes =>
            entities.Include(ph => ph.Merch).ThenInclude(m => m.Shop).
            ThenInclude(s => s.Merches).ThenInclude(m => m.PriceHistory).
            ThenInclude(ph => ph.TimestampedPrices);
    }
}
