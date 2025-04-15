using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.Entities;
using PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base;
using PriceTracker.Models.DomainModels;
using System.Runtime.CompilerServices;
using PriceTracker.Models.DataAccess.Mapping.Extensions;

using MerchMap = PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base.IDomainToEntityMapper
    <PriceTracker.Models.DomainModels.MerchModel, PriceTracker.Models.DataAccess.Entities.MerchEntity>;
using ShopMap = PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base.IDomainToEntityMapper
    <PriceTracker.Models.DomainModels.ShopModel, PriceTracker.Models.DataAccess.Entities.ShopEntity>;
using PriceHistoryMap = PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base.IDomainToEntityMapper
    <PriceTracker.Models.DomainModels.MerchPriceHistory, PriceTracker.Models.DataAccess.Entities.MerchPriceHistoryEntity>;
using TimestampedPriceMap = PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base.IDomainToEntityMapper
    <PriceTracker.Models.DomainModels.TimestampedPrice, PriceTracker.Models.DataAccess.Entities.TimestampedPriceEntity>;



/*
 /// <summary>
        /// entity при попадании в метод должен впитать, отмаппить в себя содержимое model.
        /// Не реализуй новых инстансов в методе.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        protected abstract void ModelInwardsToEntity(TEntity entity, TDomain domain);

        /// <summary>
        /// С чистого листа создаётся entity на основе model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected abstract TEntity CreateEntityFromModel(TDomain model);

        /// <summary>
        /// Создание инстанса Model на основе entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected abstract TDomain CreateDomainFromEntity(TEntity entity);
 
 */


// TODO: [Допущение] предполагается маппинг (и соответственно - загрузка в репозитории) всех навигационных свойств.


namespace PriceTracker.Models.DataAccess.Mapping
{
    public class EntityToModelMappingContext: MerchMap, ShopMap, PriceHistoryMap, TimestampedPriceMap
    {

        protected readonly DbContext _dbContext;

        private readonly DbSet<MerchEntity> merches;
        private readonly DbSet<ShopEntity> shops;
        private readonly DbSet<TimestampedPriceEntity> timestampedPrices;
        private readonly DbSet<MerchPriceHistoryEntity> merchPriceHistories;

        // TODO: [Важно] Важно гарантировать единичность всех инстансов TDomain'а.
        // TOFIX: ConditionalWeakTable не гарантирует единичность всех инстансов:
        // при удалении Entity, Model может остаться, и быть вновь созданной и внесенной в таблицу.
        private readonly ConditionalWeakTable<MerchEntity, MerchModel> _merchMapCache;
        private readonly ConditionalWeakTable<ShopEntity, ShopModel> _shopMapCache;
        private readonly ConditionalWeakTable<TimestampedPriceEntity, TimestampedPrice> _timestampedPriceMapCache;
        private readonly ConditionalWeakTable<MerchPriceHistoryEntity, MerchPriceHistory> _priceHistoryMapCache;

        public EntityToModelMappingContext(DbContext dbContext)
        {
            _dbContext = dbContext;
            merches = _dbContext.Set<MerchEntity>();
            shops = _dbContext.Set<ShopEntity>();
            timestampedPrices = _dbContext.Set<TimestampedPriceEntity>();
            merchPriceHistories = _dbContext.Set<MerchPriceHistoryEntity>();

            _merchMapCache = [];
            _shopMapCache = [];
            _timestampedPriceMapCache = [];
            _priceHistoryMapCache = [];
        }

        public MerchEntity Map(MerchModel model)
        {
            return ModelToEntity(model, merches, ModelInwardsToEntity, CreateEntityFromModel);
        }
        public ShopEntity Map(ShopModel model)
        {
            return ModelToEntity(model, shops, ModelInwardsToEntity, CreateEntityFromModel);
        }
        public TimestampedPriceEntity Map(TimestampedPrice model)
        {
            return ModelToEntity(model, timestampedPrices, ModelInwardsToEntity, CreateEntityFromModel);
        }
        public MerchPriceHistoryEntity Map(MerchPriceHistory model)
        {
            return ModelToEntity(model, merchPriceHistories, ModelInwardsToEntity, CreateEntityFromModel);
        }

        public MerchModel Map(MerchEntity entity)
        {
            return EntityToModel(entity, _merchMapCache, CreateDomainFromEntity);
        }
        public ShopModel Map(ShopEntity entity)
        {
            return EntityToModel(entity, _shopMapCache, CreateDomainFromEntity);
        }
        public TimestampedPrice Map(TimestampedPriceEntity entity)
        {
            return EntityToModel(entity, _timestampedPriceMapCache, CreateDomainFromEntity);
        }
        public MerchPriceHistory Map(MerchPriceHistoryEntity entity)
        {
            return EntityToModel(entity, _priceHistoryMapCache, CreateDomainFromEntity);
        }

        // TODO: Выглядит костыльно. Перепроверить на соответствие SOLID - особенно Open closed.
        // TODO: Не все свойства маппятся, т.к. не везде есть полное по ним соответствие.

        protected void ModelInwardsToEntity(MerchEntity entity, MerchModel domain)
        {
            entity.Name = domain.Name;
            entity.Shop = Map(domain.Shop);
            entity.PriceHistory = Map(domain.PriceTrack);
        }
        protected void ModelInwardsToEntity(ShopEntity entity, ShopModel domain)
        {
            entity.Name = domain.Name;
            entity.Merches = ((MerchMap)this).MapMany(domain.Merches);
        }
        protected void ModelInwardsToEntity(TimestampedPriceEntity entity, TimestampedPrice domain)
        {
            entity.Price = domain.Price;
            entity.DateTime = domain.DateTime;
        }
        protected void ModelInwardsToEntity(MerchPriceHistoryEntity entity, MerchPriceHistory domain)
        {
            entity.CurrentPrice = Map(domain.CurrentPrice);
            entity.TimestampedPrices = ((TimestampedPriceMap)this).MapMany(domain.TimestampedPrices.ToList());
        }

        protected MerchEntity CreateEntityFromModel(MerchModel model)
        {
            MerchEntity merchEntity = new(model.Name, model.Id);
            merchEntity.Shop = Map(model.Shop);
            merchEntity.PriceHistory = Map(model.PriceTrack);
            return merchEntity;
        }
        protected ShopEntity CreateEntityFromModel(ShopModel model)
        {
            ShopEntity entity = new(model.Name, model.Id);
            entity.Merches = ((MerchMap)this).MapMany(model.Merches) ?? [];
            return entity;
        }
        protected TimestampedPriceEntity CreateEntityFromModel(TimestampedPrice model)
        {
            return new(model.Price, model.DateTime, model.Id);
        }
        protected MerchPriceHistoryEntity CreateEntityFromModel(MerchPriceHistory model)
        {
            var entity = new MerchPriceHistoryEntity(model.Id);
            entity.TimestampedPrices = ((TimestampedPriceMap)this).MapMany(model.TimestampedPrices.ToList());
            entity.CurrentPrice = Map(model.CurrentPrice);
            return entity;
        }

        protected MerchModel CreateDomainFromEntity(MerchEntity entity)
        {
            return new(entity.Name, Map(entity.PriceHistory), Map(entity.Shop), entity.Id);
        }
        protected ShopModel CreateDomainFromEntity(ShopEntity entity)
        {
            return new(entity.Name, ((MerchMap)this).MapMany(entity.Merches), entity.Id);
        }
        protected TimestampedPrice CreateDomainFromEntity(TimestampedPriceEntity entity)
        {
            return new(entity.Price, entity.DateTime, entity.Id);
        }
        protected MerchPriceHistory CreateDomainFromEntity(MerchPriceHistoryEntity entity)
        {
            return new(Map(entity.CurrentPrice), ((TimestampedPriceMap)this).MapMany(entity.TimestampedPrices), entity.Id);
        }


        protected static TEntity ModelToEntity<TEntity, TDomain>(TDomain domainModel, DbSet<TEntity> set, 
            Action<TEntity, TDomain> modelInwardsToEntity, Func<TDomain, TEntity> createEntityFromModel)
            where TEntity : BaseEntity where TDomain: BaseModel
        {
            var entity = set.SingleOrDefault(e => e.Id == domainModel.Id);
            if (entity != null)
            {
                modelInwardsToEntity(entity, domainModel);
            }
            else
            {
                entity = createEntityFromModel(domainModel);
            }
            return entity;
        }
        protected static TDomain EntityToModel<TEntity, TDomain>(TEntity entity, ConditionalWeakTable<TEntity, TDomain> cache,
            Func<TEntity, TDomain> createDomainFromEntity)
            where TEntity : BaseEntity where TDomain : BaseModel
        {
            if (cache.TryGetValue(entity, out var result))
                return result;
            else
            {
                var model = createDomainFromEntity(entity);
                cache.AddOrUpdate(entity, model);
                return model;
            }
        }
    }
}
