using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;
using PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base;

namespace PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Common.Domain
{
    public class PriceHistoryMapper : BidirectionalDomainEntityMapper
        <MerchPriceHistory, MerchPriceHistoryEntity>
    {
        Func<TimestampedPrice, TimestampedPriceEntity> TimestampedPriceDomainToEntity;
        Func<TimestampedPriceEntity, TimestampedPrice> TimestampedPriceEntityToDomain;


        public PriceHistoryMapper(DbContext dbContext,
            Func<TimestampedPrice, TimestampedPriceEntity> timestampedPriceDomainToEntity,
            Func<TimestampedPriceEntity, TimestampedPrice> timestampedPriceEntityToDomain) :
            base(dbContext)
        {
            TimestampedPriceDomainToEntity = timestampedPriceDomainToEntity;
            TimestampedPriceEntityToDomain = timestampedPriceEntityToDomain;
        }



        protected override void MapModelFieldsToEntity(MerchPriceHistoryEntity entity, MerchPriceHistory domain)
        {
            entity.CurrentPrice = TimestampedPriceDomainToEntity(domain.CurrentPrice);
            entity.TimestampedPrices = domain.TimestampedPrices.Select(TimestampedPriceDomainToEntity).ToList();
        }

        protected override MerchPriceHistoryEntity CreateEntityFromDomain(MerchPriceHistory domain)
        {
            var entity = new MerchPriceHistoryEntity(domain.Id);
            entity.TimestampedPrices = domain.TimestampedPrices.Select(TimestampedPriceDomainToEntity).ToList();
            entity.CurrentPrice = TimestampedPriceDomainToEntity(domain.CurrentPrice);
            return entity;
        }


        protected override MerchPriceHistory CreateDomainFromEntity(MerchPriceHistoryEntity entity)
        {
            return new(TimestampedPriceEntityToDomain(entity.CurrentPrice),
                entity.TimestampedPrices.Select(TimestampedPriceEntityToDomain).ToList(), entity.Id);
        }

    }
}
