using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.Entities;
using PriceTracker.Models.DataAccess.Entities.Process.ShopSpecific;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base;
using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DomainModels.ShopSpecificModels.Citilink;

namespace PriceTracker.Models.DataAccess.Mapping.ShopSpecific.Citilink
{
    public class CitilinkMappingContext: BaseDecoratorForBidirectionalEntityModelMappingContext,
        IBidirectionalEntityModelMappingContext,
        IBidirectionalDomainEntityMapper<CitilinkMerch, CitilinkMerchEntity>
    {
        private readonly CitilinkMerchMapper _citilinkMerchMapper;
        public CitilinkMerch Map(CitilinkMerchEntity entity)
        {
            return _citilinkMerchMapper.Map(entity);
        }

        public CitilinkMerchEntity Map(CitilinkMerch domain)
        {
            return _citilinkMerchMapper.Map(domain);
        }

        protected CitilinkMerch CreateDomainFromEntity(CitilinkMerchEntity entity)
        {
            return new(entity.Name, Map(entity.PriceHistory), Map(entity.Shop), entity.CitilinkId,
                entity.Id);
        }

        public CitilinkMappingContext(DbContext dbContext, 
            IBidirectionalEntityModelMappingContext baseMappingContext):
            base(baseMappingContext)
        {
            _citilinkMerchMapper = new(dbContext, baseMappingContext.Map,
                baseMappingContext.Map, baseMappingContext.Map, baseMappingContext.Map);
        }
    }
}
