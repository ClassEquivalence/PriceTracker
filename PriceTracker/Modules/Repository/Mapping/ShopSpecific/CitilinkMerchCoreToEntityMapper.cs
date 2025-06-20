using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Entities.Domain.ShopSpecific;
using PriceTracker.Modules.Repository.Mapping.Domain;
using System.Reflection.Metadata.Ecma335;

namespace PriceTracker.Modules.Repository.Mapping.ShopSpecific
{
    public class CitilinkMerchCoreToEntityMapper : BaseGenericCoreToEntityMapper<CitilinkMerchDto, CitilinkMerchEntity>,
        IMerchCoreToEntityMapper<CitilinkMerchDto, CitilinkMerchEntity>
    {
        private readonly IPriceHistoryMapper _priceHistoryMapper;
        public CitilinkMerchCoreToEntityMapper(IPriceHistoryMapper priceHistoryMapper,
            Func<CitilinkMerchDto, CitilinkMerchEntity?> getEntityIfExists) : base(getEntityIfExists)
        {
            _priceHistoryMapper = priceHistoryMapper;
        }

        protected override CitilinkMerchEntity CreateEntityFromModel(CitilinkMerchDto model)
        {
            CitilinkMerchEntity entity = new(model.Name, model.CitilinkId, model.Id);
            entity.ShopId = model.ShopId;
            entity.PriceHistoryId = model.PriceHistoryId;
            return new CitilinkMerchEntity(model.Name, model.CitilinkId, model.Id);
        }

        protected override CitilinkMerchDto CreateModelFromEntity(CitilinkMerchEntity entity)
        {
            return new CitilinkMerchDto(entity.Id, entity.Name, _priceHistoryMapper.Map(entity.PriceHistory),
                entity.CitilinkId, entity.ShopId, entity.PriceHistoryId);
        }
    }
}
