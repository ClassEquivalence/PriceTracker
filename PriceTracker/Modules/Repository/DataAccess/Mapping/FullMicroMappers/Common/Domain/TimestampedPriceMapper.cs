using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;
using PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base;

namespace PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Common.Domain
{
    public class TimestampedPriceMapper : BidirectionalDomainEntityMapper
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
