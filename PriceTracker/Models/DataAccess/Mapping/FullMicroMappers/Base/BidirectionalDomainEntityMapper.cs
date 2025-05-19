using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.Entities.Domain;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DataAccess.Mapping.FullMicroMappers.Base
{


    // TODO: В целях масштабируемости можно выделить ветку иерархии,
    // отвечающую не за создание новых моделей, а за маппинг всех свойств
    // (на основе и по аналогии с MapModelFieldsToEntity).
    public abstract class BidirectionalDomainEntityMapper<TDomain, TEntity>: 
        IBidirectionalDomainEntityMapper<TDomain, TEntity>
        where TDomain : BaseDomain where TEntity: BaseEntity
    {
        protected readonly DbContext DbContext;

        // TODO: [Важно] Важно гарантировать единичность всех инстансов TDomain'а.
        protected readonly Dictionary<int, WeakReference<TDomain>> Cache;

        protected readonly DbSet<TEntity> Entities;

        public BidirectionalDomainEntityMapper(DbContext dbContext)
        {
            this.DbContext = dbContext;
            Entities = dbContext.Set<TEntity>();

            Cache = [];
        }
        public BidirectionalDomainEntityMapper(DbContext dbContext,
            Dictionary<int, WeakReference<TDomain>> cache)
        {
            this.DbContext = dbContext;
            Entities = dbContext.Set<TEntity>();

            Cache = cache;
        }

        /// <summary>
        /// Маппит все поля модели в сущность (кроме Id).
        /// </summary>
        protected abstract void MapModelFieldsToEntity(TEntity entity,
            TDomain domain);

        /// <summary>
        /// Создает новый экземпляр объекта в семантике бизнес-логики (Domain).
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected abstract TDomain CreateDomainFromEntity(TEntity entity);

        /// <summary>
        /// Создает новый экземпляр объекта в семантике сущности (EF Entity).
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        protected abstract TEntity CreateEntityFromDomain(TDomain domain);


        public TEntity Map(TDomain domain)
        {
            var entity = Entities.SingleOrDefault(e => e.Id == domain.Id);
            if (entity != null)
            {
                MapModelFieldsToEntity(entity, domain);
            }
            else
            {
                entity = CreateEntityFromDomain(domain);
            }
            return entity;
        }

        public TDomain Map(TEntity entity)
        {
            TDomain? domain;
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
            domain = CreateDomainFromEntity(entity);
            SetDomainCache(domain);

            return domain;
        }

        protected void SetDomainCache(TDomain domain)
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
