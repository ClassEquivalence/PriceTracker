using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.EFCore;
using PriceTracker.Models.DataAccess.Entities.Domain;
using PriceTracker.Models.DataAccess.Mapping;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Repositories
{
    public class TimestampedPriceRepository: EFGenericRepository<TimestampedPrice, TimestampedPriceEntity>
    {
        private readonly IBidirectionalDomainEntityMapper<TimestampedPrice, TimestampedPriceEntity> _mappingContext;

        protected override List<TimestampedPriceEntity> listOfEntities
        {
            get
            {
                return entities.Include(p=>p.MerchPriceHistory).ThenInclude(h=>h.Merch).ToList();
            }
        }


        public TimestampedPriceRepository(PriceTrackerContext context, BidirectionalEntityModelMappingContext mappingContext) : base(context)
        {
            _mappingContext = mappingContext;
        }

        protected override TimestampedPrice EntityToModel(TimestampedPriceEntity entity) => _mappingContext.Map(entity);
        protected override TimestampedPriceEntity ModelToEntity(TimestampedPrice model) => _mappingContext.Map(model);
    }
}
