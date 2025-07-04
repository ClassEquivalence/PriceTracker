using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.Entities.Domain.MerchPriceHistory;
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
            entities.Include(ph => ph.Merch).Include(ph => ph.CurrentPricePointer).
            ThenInclude(cpp => cpp.CurrentPrice).Include(ph => ph.TimestampedPrices);
    }
}
