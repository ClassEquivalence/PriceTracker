using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models.Infrastructure;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities.Infrastructure;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction;
using PriceTracker.Modules.Repository.Mapping;
using PriceTracker.Modules.Repository.Repositories.Base.SingletonRepository;

namespace PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink
{
    public class CitilinkExtractorStorageStateRepository: 
        EFGenericSingletonRepository<CitilinkExtractorStorageStateDto,
            CitilinkExtractorStorageStateEntity>
    {

        public CitilinkExtractorStorageStateRepository(IDbContextFactory<PriceTrackerContext>
            factory, ICoreToEntityMapper<CitilinkExtractorStorageStateDto,
                CitilinkExtractorStorageStateEntity> mapper):
            base(factory, mapper)
        {

        }

        protected override IQueryable<CitilinkExtractorStorageStateEntity> 
            GetWithIncludedEntities(DbSet<CitilinkExtractorStorageStateEntity> entities)
        {
            return entities;
        }
    }
}
