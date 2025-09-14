using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.Entities;

namespace PriceTracker.Modules.Repository.Repositories.Base
{


    public abstract class EFGenericEntityRepository<TEntity, SpecificDbContext> :
        IEntityRepository<TEntity>
        where TEntity : BaseEntity
        where SpecificDbContext : DbContext
    {

        protected ILogger? Logger;

        protected virtual IDbContextFactory<SpecificDbContext> _dbContextFactory
        { get; set; }

        //TODO: Прописать инклюды всякие.

        public EFGenericEntityRepository(IDbContextFactory<SpecificDbContext> dbContextFactory,
            ILogger? logger = null)
        {
            _dbContextFactory = dbContextFactory;
            Logger = logger;
        }


        public List<TEntity> Where(Func<TEntity, bool>? predicate = null)
        {
            predicate ??= ((e) => true);
            using SpecificDbContext context =
                _dbContextFactory.CreateDbContext();
            return GetWithIncludedEntities(context).Where(predicate).ToList();
        }
        public bool Any(Func<TEntity, bool>? predicate = null)
        {
            predicate ??= ((e) => true);
            using SpecificDbContext context =
                _dbContextFactory.CreateDbContext();
            return GetWithIncludedEntities(context).Any(predicate);
        }

        public void Create(TEntity entity)
        {
            using SpecificDbContext context =
                _dbContextFactory.CreateDbContext();
            context.Add(entity);
            context.SaveChanges();
        }
        public virtual bool Update(TEntity entity)
        {
            using SpecificDbContext context =
                _dbContextFactory.CreateDbContext();

            context.Update(entity);
            context.SaveChanges();

            // TODO: Возврат - бессмысленная заглушка.
            return true;
        }
        public bool Delete(int id)
        {
            using SpecificDbContext context =
                _dbContextFactory.CreateDbContext();

            var e = GetEntity(context, id);
            if (e != null && GetWithIncludedEntities(context).Contains(e))
            {
                context.Remove(e);
                context.SaveChanges();
                return true;
            }
            else
                return false;
        }
        public void SaveChanges()
        {
            // TODO: Пустая заглушка. Хотелось бы, чтобы она однако была функциональной,
            // либо убрать.
            //dbContext.SaveChanges();
        }

        /// <summary>
        /// Выбирается Entity с указанным id. Возвращает null, если элемент с указаным id не найден.
        /// Выбрасывает исключение, если в последовательности имеется не менее 2 объектов с одинаковым Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public TEntity? GetEntity(int id)
        {
            using var context = _dbContextFactory.CreateDbContext();

            return GetEntity(context, id);
        }


        private TEntity? GetEntity(SpecificDbContext context, int id)
        {
            return GetWithIncludedEntities(context).SingleOrDefault(e => e.Id == id);
        }


        protected IQueryable<TEntity> GetWithIncludedEntities(SpecificDbContext context)
        {
            var set = context.Set<TEntity>();
            return GetWithIncludedEntities(set);
        }



        /// <summary>
        /// Метод создан для инклюда всех связанных сущностей.
        /// Возвращаемый IQueryable должно находиться в состоянии уже после инклюдов.
        /// </summary>
        protected abstract IQueryable<TEntity> GetWithIncludedEntities(DbSet<TEntity> entities);
    }
}
