using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Entities.Domain.MerchPriceHistory;

namespace PriceTracker.Modules.Repository.Mapping.Domain
{
    // TODO: Не маппится из dto в entity свойство Shop.
    public class PriceHistoryMapper : BaseGenericCoreToEntityMapper<MerchPriceHistoryDto,
        MerchPriceHistoryEntity>, IPriceHistoryMapper
    {
        private readonly ITimestampedPriceMapper _timestampedPriceMapper;

        private readonly ILogger? _logger;

        public PriceHistoryMapper(ITimestampedPriceMapper timestampedPriceMapper,
            ILogger? logger = null) : base()
        {
            _logger = logger;
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

            var currentPricePointerAsEntity = new CurrentPricePointerEntity(entity.Id, 
                currentPriceAsEntity.Id, model.CurrentPricePointerId);

            currentPricePointerAsEntity.MerchPriceHistory = entity;
            currentPricePointerAsEntity.CurrentPrice = currentPriceAsEntity;

            entity.TimestampedPrices = timestampedPricesAsEntities;
            entity.CurrentPricePointer = currentPricePointerAsEntity;
            entity.MerchId = model.MerchId;

            return entity;
        }

        protected override MerchPriceHistoryDto CreateModelFromEntity(MerchPriceHistoryEntity entity)
        {
            var currentPriceAsEntity = entity.CurrentPricePointer.CurrentPrice;
            var previousPricesAsEntities = entity.TimestampedPrices.
                Where(p => p.Id != currentPriceAsEntity.Id);
            var previousPricesAsCoreDtos = previousPricesAsEntities.Select
                (ppae => _timestampedPriceMapper.Map(ppae)).ToList();
            var currentPriceAsCoreDto = _timestampedPriceMapper.Map(currentPriceAsEntity);
            return new MerchPriceHistoryDto(entity.Id, previousPricesAsCoreDtos, currentPriceAsCoreDto,
                entity.MerchId, entity.CurrentPricePointer.Id);
        }

        protected override void MapModelFieldsToEntity(MerchPriceHistoryDto model, MerchPriceHistoryEntity entity)
        {
            entity.CurrentPricePointer.CurrentPrice = _timestampedPriceMapper.Map(model.CurrentPrice);
            entity.MerchId = model.MerchId;
            entity.TimestampedPrices = model.PreviousTimestampedPricesList.Select(_timestampedPriceMapper.Map)
                .ToList();
        }
    }
}
