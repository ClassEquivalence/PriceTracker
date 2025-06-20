using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Entities.Domain;

namespace PriceTracker.Modules.Repository.Mapping.Domain
{
    // TODO: Не маппится из dto в entity свойство Shop.
    public class PriceHistoryMapper : BaseGenericCoreToEntityMapper<MerchPriceHistoryDto,
        MerchPriceHistoryEntity>, IPriceHistoryMapper
    {
        private readonly ITimestampedPriceMapper _timestampedPriceMapper;

        public PriceHistoryMapper(ITimestampedPriceMapper timestampedPriceMapper,
            Func<MerchPriceHistoryDto, MerchPriceHistoryEntity?> getEntityIfExists) : base(getEntityIfExists)
        {
            _timestampedPriceMapper = timestampedPriceMapper;
        }

        protected override MerchPriceHistoryEntity CreateEntityFromModel(MerchPriceHistoryDto model)
        {
            MerchPriceHistoryEntity entity = new(model.Id);
            var currentPriceAsCoreDto = model.CurrentPrice;
            var previousPricesAsCoreDtos = model.PreviousTimestampedPricesList;

            var currentPriceAsEntity = _timestampedPriceMapper.Map(currentPriceAsCoreDto);
            var timestampedPricesAsEntities = previousPricesAsCoreDtos.
                Select(pe => _timestampedPriceMapper.Map(pe)).Append(currentPriceAsEntity).ToList();

            entity.TimestampedPrices = timestampedPricesAsEntities;
            entity.CurrentPrice = currentPriceAsEntity;
            entity.MerchId = model.MerchId;

            return entity;
        }

        protected override MerchPriceHistoryDto CreateModelFromEntity(MerchPriceHistoryEntity entity)
        {
            var currentPriceAsEntity = entity.CurrentPrice;
            var previousPricesAsEntities = entity.TimestampedPrices.
                Where(p => p.Id != currentPriceAsEntity.Id);
            var previousPricesAsCoreDtos = previousPricesAsEntities.Select
                (ppae => _timestampedPriceMapper.Map(ppae)).ToList();
            var currentPriceAsCoreDto = _timestampedPriceMapper.Map(currentPriceAsEntity);
            return new MerchPriceHistoryDto(entity.Id, previousPricesAsCoreDtos, currentPriceAsCoreDto,
                entity.MerchId);
        }
    }
}
