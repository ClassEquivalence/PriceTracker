using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models.Infrastructure;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Infrastructure;
using PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Repositories.Base.SingletonRepository;

namespace PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink
{
    public class CitilinkExtractorStorageStateRepository :
        EFGenericSingletonRepository<CitilinkExtractorStorageStateDto,
            CitilinkExtractorStorageStateEntity>
    {

        public CitilinkExtractorStorageStateRepository(IDbContextFactory<PriceTrackerContext>
            factory, ICitilinkExtractorStorageStateMapper mapper) :
            base(factory, mapper)
        {
        }

        protected override async Task<IQueryable<CitilinkExtractorStorageStateEntity>>
            GetWithIncludedEntitiesAsync()
        {
            var context = await _dbContextFactory.CreateDbContextAsync();
            return context.Set<CitilinkExtractorStorageStateEntity>();
        }
    }
}
