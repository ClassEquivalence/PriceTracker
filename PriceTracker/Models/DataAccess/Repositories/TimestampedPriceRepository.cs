using PriceTracker.Models.DataAccess.EFCore;
using PriceTracker.Models.DataAccess.Entities;
using PriceTracker.Models.DataAccess.Mapping;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Repositories
{
    public class TimestampedPriceRepository: EFGenericRepository<TimestampedPrice, TimestampedPriceEntity>
    {
        private readonly IDomainToEntityMapper<TimestampedPrice, TimestampedPriceEntity> _mappingContext;

        public TimestampedPriceRepository(PriceTrackerContext context, EntityToModelMappingContext mappingContext) : base(context)
        {
            _mappingContext = mappingContext;
        }

        protected override TimestampedPrice EntityToModel(TimestampedPriceEntity entity) => _mappingContext.Map(entity);
        protected override TimestampedPriceEntity ModelToEntity(TimestampedPrice model) => _mappingContext.Map(model);
    }
}
