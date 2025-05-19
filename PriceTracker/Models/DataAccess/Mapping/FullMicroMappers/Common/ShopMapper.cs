using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.Entities.Domain;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Common
{
    public class ShopMapper: BidirectionalDomainEntityMapper<ShopModel, ShopEntity>
    {

        Func<MerchModel, MerchEntity> MerchModelToEntity;
        Func<MerchEntity, MerchModel> MerchEntityToModel;

        public ShopMapper(DbContext dbContext, 
            Func<MerchModel, MerchEntity> merchModelToEntity,
            Func<MerchEntity, MerchModel> merchEntityToModel) : base(dbContext)
        {
            MerchModelToEntity = merchModelToEntity;
            MerchEntityToModel = merchEntityToModel;
        }

        protected override void MapModelFieldsToEntity(ShopEntity entity, ShopModel domain)
        {
            entity.Name = domain.Name;
            entity.Merches = domain.Merches.Select(MerchModelToEntity).ToList();
        }
        protected override ShopEntity CreateEntityFromDomain(ShopModel domain)
        {
            ShopEntity entity = new(domain.Name, domain.Id);
            entity.Merches = domain.Merches.Select(MerchModelToEntity).ToList() ?? [];
            return entity;
        }
        protected override ShopModel CreateDomainFromEntity(ShopEntity entity)
        {
            return new(entity.Name, entity.Merches.Select(MerchEntityToModel).ToList(),
                entity.Id);
        }
    }
}
