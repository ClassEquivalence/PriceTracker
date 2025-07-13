using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Domain.MerchPriceHistory;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.EntityLevel
{
    public class PriceHistoryEntityRepository : EFGenericEntityRepository
        <MerchPriceHistoryEntity, PriceTrackerContext>
    {
        public PriceHistoryEntityRepository(IDbContextFactory<PriceTrackerContext>
            dbContextFactory) : base(dbContextFactory)
        {
        }

        protected override IQueryable<MerchPriceHistoryEntity>
            GetWithIncludedEntities(DbSet<MerchPriceHistoryEntity> entities)
        {
            return entities.Include(ph => ph.Merch).Include(ph => ph.CurrentPricePointer).
            ThenInclude(cpp => cpp.CurrentPrice).Include(ph => ph.TimestampedPrices);
        }
    }
}
