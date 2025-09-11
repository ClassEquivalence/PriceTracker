using Microsoft.EntityFrameworkCore;
using PriceTracker.Core.Models;
using PriceTracker.Modules.Repository.DataAccess.EFCore;
using PriceTracker.Modules.Repository.Entities;
using PriceTracker.Modules.Repository.Mapping;

namespace PriceTracker.Modules.Repository.Repositories.Base.SingletonRepository
{
    public abstract class EFGenericSingletonRepository<Dto, TEntity> : ISingletonRepository<Dto>
        where Dto : BaseDto where TEntity: BaseEntity
    {
        protected readonly IDbContextFactory<PriceTrackerContext> _dbContextFactory;
        protected readonly ICoreToEntityMapper<Dto, TEntity> _mapper;

        public EFGenericSingletonRepository(IDbContextFactory<PriceTrackerContext>
            contextFactory, ICoreToEntityMapper<Dto, TEntity> mapper)
        {
            _dbContextFactory = contextFactory;
            _mapper = mapper;

            var isEnsured = EnsureEntitiesNoMoreThanOne();

            if (!isEnsured)
            {
                throw new InvalidOperationException($"Строк {nameof(TEntity)}" +
                    "в БД должно быть не более одной.");
            }
            
        }

        public Dto? Get()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var set = context.Set<TEntity>();
            var entities = GetWithIncludedEntities(set);

            if (!(entities.Count() == 1))
                return null;
            else
                return _mapper.Map(entities.SingleOrDefault());
        }

        public virtual void Set(Dto dto)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var set = context.Set<TEntity>();
            var entity = _mapper.Map(dto);

            using var context_second = _dbContextFactory.CreateDbContext();

            var count = set.Count();
            if (count == 1) 
            {
                entity.Id = set.Single().Id;
                context_second.Set<TEntity>().Update(entity);
                context_second.SaveChanges();
            }
            else if(count == 0)
            {
                context_second.Set<TEntity>().Add(entity);
                context_second.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException($"{nameof(EFGenericSingletonRepository<Dto, TEntity>)}, " +
                    $"{nameof(Set)}: \n" +
                    $"Синглтон-сущностей типа {nameof(TEntity)} почему-то не 1 и не 0.");
            }
        }

        public bool Any()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var set = context.Set<TEntity>();
            return set.Any();
        }


        protected virtual bool EnsureEntitiesNoMoreThanOne()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var set = context.Set<TEntity>();

            var count = set.Count();

            if (count <= 1)
                return true;
            else
                return false;
        }

        protected abstract IQueryable<TEntity> GetWithIncludedEntities
            (DbSet<TEntity> entities);
    }
}
