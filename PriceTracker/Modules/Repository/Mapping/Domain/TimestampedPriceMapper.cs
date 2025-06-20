using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Entities.Domain;

namespace PriceTracker.Modules.Repository.Mapping.Domain
{
    public class TimestampedPriceMapper : BaseGenericCoreToEntityMapper<TimestampedPriceDto, 
        TimestampedPriceEntity>, ITimestampedPriceMapper
    {
        public TimestampedPriceMapper(Func<TimestampedPriceDto, TimestampedPriceEntity?> 
            getEntityIfExists) : base(getEntityIfExists)
        {

        }
        

        protected override TimestampedPriceEntity CreateEntityFromModel(TimestampedPriceDto model)
        {
            TimestampedPriceEntity entity = new(model.Price, model.DateTime, model.Id);
            entity.MerchPriceHistoryId = model.MerchPriceHistoryId;
            return entity;
        }

        protected override TimestampedPriceDto CreateModelFromEntity(TimestampedPriceEntity entity)
        {
            return new TimestampedPriceDto(entity.Id, entity.Price, entity.DateTime,
                entity.MerchPriceHistoryId);
        }
    }
}
