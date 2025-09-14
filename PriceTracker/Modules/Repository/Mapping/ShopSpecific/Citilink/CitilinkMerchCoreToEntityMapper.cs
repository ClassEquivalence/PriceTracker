using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Entities.Domain.ShopSpecific;
using PriceTracker.Modules.Repository.Mapping.Domain;

namespace PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink
{
    public class CitilinkMerchCoreToEntityMapper : BaseGenericCoreToEntityMapper<CitilinkMerchDto, CitilinkMerchEntity>,
        IMerchCoreToEntityMapper<CitilinkMerchDto, CitilinkMerchEntity>
    {
        private readonly IPriceHistoryMapper _priceHistoryMapper;
        public CitilinkMerchCoreToEntityMapper(IPriceHistoryMapper priceHistoryMapper) : base()
        {
            _priceHistoryMapper = priceHistoryMapper;
        }

        protected override CitilinkMerchEntity CreateEntityFromModel(CitilinkMerchDto model)
        {
            CitilinkMerchEntity entity = new(model.Name, model.CitilinkId, model.Id);
            entity.ShopId = model.ShopId;
            entity.PriceHistory = _priceHistoryMapper.Map(model.PriceTrack);
            entity.PriceHistory.Id = model.PriceHistoryId;
            return entity;
        }

        protected override CitilinkMerchDto CreateModelFromEntity(CitilinkMerchEntity entity)
        {
            return new CitilinkMerchDto(entity.Id, entity.Name, _priceHistoryMapper.Map(entity.PriceHistory),
                entity.CitilinkId, entity.ShopId, entity.PriceHistory.Id);
        }

        protected override void MapModelFieldsToEntity(CitilinkMerchDto model, CitilinkMerchEntity entity)
        {
            entity.Name = model.Name;
            entity.PriceHistory = _priceHistoryMapper.Map(model.PriceTrack);
            entity.ShopId = model.ShopId;
            entity.CitilinkId = model.CitilinkId;

        }
    }
}
