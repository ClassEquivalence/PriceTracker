using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;
using PriceTracker.Modules.Repository.DataAccess.Mapping;
using PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base;

namespace PriceTracker.Modules.Repository.DataAccess.Repositories
{
    public class TimestampedPriceRepository : EFGenericRepository<TimestampedPrice, TimestampedPriceEntity>
    {
        private readonly IBidirectionalDomainEntityMapper<TimestampedPrice, TimestampedPriceEntity> _mappingContext;

        protected override List<TimestampedPriceEntity> listOfEntities
        {
            get
            {
                return entities.Include(p => p.MerchPriceHistory).ThenInclude(h => h.Merch).ToList();
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
