using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.Entities.Domain.ShopSpecific;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink
{
    public class CitilinkMerchEntityRepository :
        EFGenericEntityRepository<CitilinkMerchEntity>
    {

        protected override IQueryable<CitilinkMerchEntity> entitiesWithIncludes =>
            entities.Include(m => m.Shop).Include(m => m.PriceHistory).
            ThenInclude(ph => ph.TimestampedPrices)
            .Include(m => m.PriceHistory).
            ThenInclude(ph => ph.CurrentPricePointer);

        public CitilinkMerchEntityRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
