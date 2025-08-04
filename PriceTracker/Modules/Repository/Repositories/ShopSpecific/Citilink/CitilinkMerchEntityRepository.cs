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
            return entities.Include(m => m.PriceHistory).
            ThenInclude(ph => ph.TimestampedPrices)
            .Include(m => m.PriceHistory).
            ThenInclude(ph => ph.CurrentPricePointer).ThenInclude(cpp => cpp.CurrentPrice);
        }

        public List<CitilinkMerchEntity> GetManyByCitilinkId(int[] citilinkIds)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return GetWithIncludedEntities(context).Where(e => citilinkIds.Contains(e.Id)).ToList();
        }

        public async Task CreateManyAsync(List<CitilinkMerchEntity> citilinkMerches)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var set = context.Set<CitilinkMerchEntity>();
            await set.AddRangeAsync(citilinkMerches);
            await context.SaveChangesAsync();
        }

        public async Task UpdateManyAsync(List<CitilinkMerchEntity> citilinkMerches)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var set = context.Set<CitilinkMerchEntity>();
            set.UpdateRange(citilinkMerches);
            await context.SaveChangesAsync();
        }
    }
}
