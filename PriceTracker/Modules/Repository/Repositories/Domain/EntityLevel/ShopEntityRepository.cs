using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.EntityLevel
{
    public class ShopEntityRepository : EFGenericEntityRepository<ShopEntity>
    {

        protected override IQueryable<ShopEntity> entitiesWithIncludes =>
            entities.Include(s => s.Merches).ThenInclude(m => m.PriceHistory)
            .ThenInclude(ph => ph.CurrentPricePointer).
            Include(s => s.Merches).ThenInclude(m => m.PriceHistory)
            .ThenInclude(ph => ph.TimestampedPrices);

        public ShopEntityRepository(DbContext dbContext) : base(dbContext)
        {
        }


    }
}
