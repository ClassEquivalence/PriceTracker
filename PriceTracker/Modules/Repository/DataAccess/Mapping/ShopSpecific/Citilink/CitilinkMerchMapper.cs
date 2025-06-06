using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DomainModels.ShopSpecificModels.Citilink;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;
using PriceTracker.Modules.Repository.DataAccess.Entities.Process.ShopSpecific;
using PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base;

namespace PriceTracker.Modules.Repository.DataAccess.Mapping.ShopSpecific.Citilink
{
    public class CitilinkMerchMapper : BidirectionalDomainEntityMapper<CitilinkMerch, CitilinkMerchEntity>
    {

        protected Func<ShopModel, ShopEntity> ShopModelToEntity;
        protected Func<MerchPriceHistory, MerchPriceHistoryEntity> PriceHistoryModelToEntity;


        protected Func<ShopEntity, ShopModel> ShopEntityToModel;
        protected Func<MerchPriceHistoryEntity, MerchPriceHistory> PriceHistoryEntityToModel;

        public CitilinkMerchMapper(DbContext context, Func<ShopModel, ShopEntity> shopModelToEntity,
            Func<ShopEntity, ShopModel> shopEntityToModel,
            Func<MerchPriceHistory, MerchPriceHistoryEntity> priceHistoryModelToEntity,
            Func<MerchPriceHistoryEntity, MerchPriceHistory> priceHistoryEntityToModel) :
            base(context)
        {
            ShopModelToEntity = shopModelToEntity;
            ShopEntityToModel = shopEntityToModel;
            PriceHistoryModelToEntity = priceHistoryModelToEntity;
            PriceHistoryEntityToModel = priceHistoryEntityToModel;


        }

        protected override CitilinkMerchEntity CreateEntityFromDomain(CitilinkMerch domain)
        {
            CitilinkMerchEntity merchEntity = new(domain.Name, domain.CitilinkId, domain.Id);
            merchEntity.Shop = ShopModelToEntity(domain.Shop);
            merchEntity.PriceHistory = PriceHistoryModelToEntity(domain.PriceTrack);
            return merchEntity;
        }

        protected override CitilinkMerch CreateDomainFromEntity(CitilinkMerchEntity entity)
        {
            return new(entity.Name, PriceHistoryEntityToModel(entity.PriceHistory),
                ShopEntityToModel(entity.Shop), entity.CitilinkId, entity.Id);
        }

        /// <summary>
        /// Маппит все поля модели в сущность.
        /// </summary>
        protected override void MapModelFieldsToEntity(CitilinkMerchEntity entity,
            CitilinkMerch domain)
        {
            entity.CitilinkId = domain.CitilinkId;
            entity.Name = domain.Name;
            entity.Shop = ShopModelToEntity(domain.Shop);
            entity.PriceHistory = PriceHistoryModelToEntity(domain.PriceTrack);
        }



    }
}
