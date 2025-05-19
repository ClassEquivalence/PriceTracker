using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.Entities.Domain;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Common
{
    public class TimestampedPriceMapper: BidirectionalDomainEntityMapper
        <TimestampedPrice, TimestampedPriceEntity>
    {
        public TimestampedPriceMapper(DbContext dbContext) : base(dbContext)
        {

        }

        protected override void MapModelFieldsToEntity(TimestampedPriceEntity entity, TimestampedPrice domain)
        {
            entity.Price = domain.Price;
            entity.DateTime = domain.DateTime;
        }
        protected override TimestampedPriceEntity CreateEntityFromDomain(TimestampedPrice domain)
        {
            return new(domain.Price, domain.DateTime, domain.Id);
        }
        protected override TimestampedPrice CreateDomainFromEntity(TimestampedPriceEntity entity)
        {
            return new(entity.Price, entity.DateTime, entity.Id);
        }
    }
}
