using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.Repository.Entities.Domain;

namespace PriceTracker.Modules.Repository.Mapping.Domain
{
    public class ShopCoreToEntityMapper : BaseGenericCoreToEntityMapper<ShopDto,
        ShopEntity>, IShopMapper
    {
        public ShopCoreToEntityMapper() : base()
        {
        }

        protected override ShopEntity CreateEntityFromModel(ShopDto model)
        {
            ShopEntity e = new(model.Name, model.Id);
            return e;
        }

        protected override ShopDto CreateModelFromEntity(ShopEntity entity)
        {
            return new ShopDto(entity.Id, entity.Name);
        }

        protected override void MapModelFieldsToEntity(ShopDto model, ShopEntity entity)
        {
            entity.Name = model.Name;
        }
    }
}
