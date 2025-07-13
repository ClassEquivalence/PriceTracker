using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Domain.ShopSpecific;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink
{
    public class CitilinkMerchEntityRepository :
        EFGenericEntityRepository<CitilinkMerchEntity, PriceTrackerContext>
    {


        public CitilinkMerchEntityRepository(IDbContextFactory<PriceTrackerContext>
            dbContextFactory) : base(dbContextFactory)
        {
        }

        protected override IQueryable<CitilinkMerchEntity> GetWithIncludedEntities(DbSet<CitilinkMerchEntity> entities)
        {
            return entities.Include(m => m.Shop).Include(m => m.PriceHistory).
            ThenInclude(ph => ph.TimestampedPrices)
            .Include(m => m.PriceHistory).
            ThenInclude(ph => ph.CurrentPricePointer);
        }
    }
}
