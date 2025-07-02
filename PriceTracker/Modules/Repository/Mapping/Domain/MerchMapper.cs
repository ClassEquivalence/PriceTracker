using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Entities.Domain;

namespace PriceTracker.Modules.Repository.Mapping.Domain
{
    public class MerchMapper : BaseGenericCoreToEntityMapper<MerchDto,
        MerchEntity>, IMerchCoreToEntityMapper<MerchDto, MerchEntity>
    {
        private readonly IPriceHistoryMapper _priceHistoryMapper;
        public MerchMapper(Func<MerchDto, MerchEntity?> getEntityIfExists,
            IPriceHistoryMapper priceHistoryMapper) :
            base(getEntityIfExists)
        {
            _priceHistoryMapper = priceHistoryMapper;
        }

        protected override MerchEntity CreateEntityFromModel(MerchDto model)
        {
            MerchEntity entity = new(model.Name, model.Id);
            entity.PriceHistory = _priceHistoryMapper.Map(model.PriceTrack);
            entity.ShopId = model.ShopId;
            return entity;
        }

        protected override MerchDto CreateModelFromEntity(MerchEntity entity)
        {
            MerchDto model = new(entity.Id, entity.Name, _priceHistoryMapper.Map(entity.PriceHistory),
                entity.ShopId, entity.PriceHistory.Id);
            return model;
        }
    }
}
