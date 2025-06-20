using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Entities.Domain;

namespace PriceTracker.Modules.Repository.Mapping.Domain
{
    public class ShopCoreToEntityMapper : BaseGenericCoreToEntityMapper<ShopDto,
        ShopEntity>, IShopMapper
    {
        private readonly IMerchCoreToEntityMapper<MerchDto, MerchEntity> _merchMapper;
        public ShopCoreToEntityMapper(IMerchCoreToEntityMapper<MerchDto, MerchEntity>
            merchMapper, Func<ShopDto, ShopEntity?> getEntityIfExists) : base(getEntityIfExists)
        {
            _merchMapper = merchMapper;
        }

        protected override ShopEntity CreateEntityFromModel(ShopDto model)
        {
            ShopEntity e = new(model.Name, model.Id);
            e.Merches = model.Merches.Select(md => _merchMapper.Map(md)).ToList();
            return e;
        }

        protected override ShopDto CreateModelFromEntity(ShopEntity entity)
        {
            return new ShopDto(entity.Id, entity.Name, entity.Merches.
                Select(me => _merchMapper.Map(me)).ToList());
        }
    }
}
