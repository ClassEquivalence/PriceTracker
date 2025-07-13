using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.EntityLevel
{
    public class TimestampedPriceEntityRepository : EFGenericEntityRepository<TimestampedPriceEntity,
        PriceTrackerContext>
    {
        public TimestampedPriceEntityRepository(IDbContextFactory<PriceTrackerContext>
            dbContextFactory) : base(dbContextFactory)
        {
        }


        protected override IQueryable<TimestampedPriceEntity>
            GetWithIncludedEntities(DbSet<TimestampedPriceEntity> entities)
        {
            return entities.Include(tp => tp.MerchPriceHistory);
        }
    }
}
