using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.Entities.Domain;
using PriceTracker.Models.DataAccess.Mapping.ShopSpecific.Citilink;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Mapping
{
    public class BaseDecoratorForBidirectionalEntityModelMappingContext
        : IBidirectionalEntityModelMappingContext
    {
        private readonly IBidirectionalEntityModelMappingContext _baseMappingContext;

        public BaseDecoratorForBidirectionalEntityModelMappingContext
            (IBidirectionalEntityModelMappingContext baseMappingContext)
        {
            _baseMappingContext = baseMappingContext;
        }

        public MerchModel Map(MerchEntity entity)
        {
            return _baseMappingContext.Map(entity);
        }

        public MerchEntity Map(MerchModel domain)
        {
            return _baseMappingContext.Map(domain);
        }

        public ShopEntity Map(ShopModel domain)
        {
            return _baseMappingContext.Map(domain);
        }
        public ShopModel Map(ShopEntity entity)
        {
            return _baseMappingContext.Map(entity);
        }

        public MerchPriceHistory Map(MerchPriceHistoryEntity entity)
        {
            return _baseMappingContext.Map(entity);
        }
        public MerchPriceHistoryEntity Map(MerchPriceHistory domain)
        {
            return _baseMappingContext.Map(domain);
        }

        public TimestampedPrice Map(TimestampedPriceEntity entity)
        {
            return _baseMappingContext.Map(entity);
        }
        public TimestampedPriceEntity Map(TimestampedPrice domain)
        {
            return _baseMappingContext.Map(domain);
        }
    }
}
