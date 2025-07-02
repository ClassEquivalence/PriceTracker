using Microsoft.EntityFrameworkCore;
using PriceTracker.Modules.Repository.Entities;

namespace PriceTracker.Modules.Repository.Repositories.Base
{
    public abstract class EFGenericEntityRepository<TEntity>
        where TEntity : BaseEntity
    {
        //TODO: Прописать инклюды всякие.
        protected DbContext dbContext;
        protected DbSet<TEntity> entities { get; set; }

        /// <summary>
        /// Поле создано для инклюда всех связанных сущностей.
        /// Поле должно находиться в состоянии уже после инклюдов.
        /// </summary>
        protected abstract IQueryable<TEntity> entitiesWithIncludes { get; }


        public EFGenericEntityRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
            entities = dbContext.Set<TEntity>();
        }


        public List<TEntity> Where(Func<TEntity, bool>? predicate = null)
        {
            predicate ??= ((e) => true);
            return entitiesWithIncludes.Where(predicate).ToList();
        }
        public bool Any(Func<TEntity, bool>? predicate = null)
        {
            predicate ??= ((e) => true);
            return entitiesWithIncludes.Any(predicate);
        }

        public void Create(TEntity entity)
        {
            entities.Add(entity);
            SaveChanges();
        }
        public bool Update(TEntity entity)
        {
            entities.Update(entity);
            dbContext.SaveChanges();
            return true;
        }
        public bool Delete(int id)
        {
            var e = GetEntity(id);
            if (e != null && entities.Contains(e))
            {
                entities.Remove(e);
                SaveChanges();
                return true;
            }
            else
                return false;
        }
        public void SaveChanges()
        {
            dbContext.SaveChanges();
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
            return entitiesWithIncludes.SingleOrDefault(e => e.Id == id);
        }


    }
}
