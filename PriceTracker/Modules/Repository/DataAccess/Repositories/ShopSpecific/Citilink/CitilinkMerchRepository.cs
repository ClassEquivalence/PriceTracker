using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.Mapping.ShopSpecific.Citilink;
using PriceTracker.Models.DomainModels.ShopSpecificModels.Citilink;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.DataAccess.Entities.Process.ShopSpecific;
using PriceTracker.Modules.Repository.DataAccess.Mapping;
using PriceTracker.Modules.Repository.DataAccess.Repositories;

namespace PriceTracker.Modules.Repository.DataAccess.Repositories.ShopSpecific.Citilink
{
    public class CitilinkMerchRepository : EFGenericRepository
        <CitilinkMerch, CitilinkMerchEntity>
    {
        private readonly BidirectionalEntityModelMappingContext _mappingContext;
        public CitilinkMerchRepository(PriceTrackerContext context,
            BidirectionalEntityModelMappingContext mappingContext) :
            base(context)
        {
            _mappingContext = mappingContext;
        }
        protected override CitilinkMerchEntity ModelToEntity(CitilinkMerch model)
        {
            return _mappingContext.Map(model);
        }

        protected override CitilinkMerch EntityToModel(CitilinkMerchEntity entity)
        {
            return _mappingContext.Map(entity);
        }
        protected override List<CitilinkMerchEntity> listOfEntities
        {
            get
            {
                return entities.Include(m => m.Shop).Include(m => m.PriceHistory).
                    ThenInclude(ph => ph.TimestampedPrices).ToList();
            }
        }
    }
}
