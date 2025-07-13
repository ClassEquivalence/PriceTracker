using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Domain;
using PriceTracker.Modules.Repository.Repositories.Base;

namespace PriceTracker.Modules.Repository.Repositories.Domain.EntityLevel
{
    public class ShopEntityRepository : EFGenericEntityRepository<ShopEntity,
        PriceTrackerContext>
    {

        public ShopEntityRepository(IDbContextFactory<PriceTrackerContext>
            dbContextFactory) : base(dbContextFactory)
        {
        }

        protected override IQueryable<ShopEntity> GetWithIncludedEntities(DbSet<ShopEntity> entities)
        {
            return entities.Include(s => s.Merches).ThenInclude(m => m.PriceHistory)
            .ThenInclude(ph => ph.CurrentPricePointer).
            Include(s => s.Merches).ThenInclude(m => m.PriceHistory)
            .ThenInclude(ph => ph.TimestampedPrices);
        }
    }
}
