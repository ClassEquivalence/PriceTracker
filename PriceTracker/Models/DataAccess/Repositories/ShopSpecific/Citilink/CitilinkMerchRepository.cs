using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.EFCore;
using PriceTracker.Models.DataAccess.Entities.Process.ShopSpecific;
using PriceTracker.Models.DataAccess.Mapping.ShopSpecific.Citilink;
using PriceTracker.Models.DomainModels.ShopSpecificModels.Citilink;

namespace PriceTracker.Models.DataAccess.Repositories.ShopSpecific.Citilink
{
    public class CitilinkMerchRepository : EFGenericRepository
        <CitilinkMerch, CitilinkMerchEntity>
    {
        private readonly CitilinkMappingContext _mappingContext;
        public CitilinkMerchRepository(PriceTrackerContext context,
            CitilinkMappingContext citilinkMapping): 
            base(context)
        {
            _mappingContext = citilinkMapping;
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
