using Microsoft.EntityFrameworkCore;
using PriceTracker.Models;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;

namespace PriceTracker.Modules.Repository.DataAccess.Mapping.FullMicroMappers.Base
{
    public abstract class BidirectionalModelEntityMapper<TModel, TEntity> :
        IBidirectionalModelEntityMapper<TModel, TEntity>
        where TModel : BaseModel where TEntity : BaseEntity
    {


        protected readonly DbContext DbContext;

        // TODO: [Важно] Важно гарантировать единичность всех инстансов TModel'и.
        protected readonly Dictionary<int, WeakReference<TModel>> Cache;

        protected readonly DbSet<TEntity> Entities;

        public BidirectionalModelEntityMapper(DbContext dbContext)
        {
            DbContext = dbContext;
            Entities = dbContext.Set<TEntity>();

            Cache = [];
        }
        public BidirectionalModelEntityMapper(DbContext dbContext,
            Dictionary<int, WeakReference<TModel>> cache)
        {
            DbContext = dbContext;
            Entities = dbContext.Set<TEntity>();

            Cache = cache;
        }

        /// <summary>
        /// Маппит все поля модели в сущность (кроме Id).
        /// </summary>
        protected abstract void MapModelFieldsToEntity(TEntity entity,
            TModel model);

        /// <summary>
        /// Создает новый экземпляр объекта в семантике модели.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected abstract TModel CreateModelFromEntity(TEntity entity);

        /// <summary>
        /// Создает новый экземпляр объекта в семантике сущности (EF Entity).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected abstract TEntity CreateEntityFromModel(TModel model);


        public TEntity Map(TModel model)
        {
            var entity = Entities.SingleOrDefault(e => e.Id == model.Id);
            if (entity != null)
            {
                MapModelFieldsToEntity(entity, model);
            }
            else
            {
                entity = CreateEntityFromModel(model);
            }
            return entity;
        }

        public TModel Map(TEntity entity)
        {
            TModel? domain;
            var weakRef = Cache.GetValueOrDefault(entity.Id);
            //var weakRefGetSuccess = weakRef?.TryGetTarget(out domain);

            if (weakRef != null)
            {
                var weakRefGetSuccess = weakRef.TryGetTarget(out domain);
                if (domain != null && weakRefGetSuccess)
                {
                    return domain;
                }
            }
            domain = CreateModelFromEntity(entity);
            SetModelCache(domain);

            return domain;
        }

        protected void SetModelCache(TModel domain)
        {
            var weakRef = Cache.GetValueOrDefault(domain.Id);
            if (weakRef != null)
            {
                weakRef.SetTarget(domain);
            }
            else
                Cache.Add(domain.Id, new(domain));
        }




    }
}
