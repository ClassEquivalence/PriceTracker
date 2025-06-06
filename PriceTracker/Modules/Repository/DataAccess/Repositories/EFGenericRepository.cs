using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DomainModels;
using PriceTracker.Models.Services.Mapping;
using PriceTracker.Modules.Repository.DataAccess.Entities.Domain;
using System.Runtime.CompilerServices;

namespace PriceTracker.Modules.Repository.DataAccess.Repositories
{

    /*
     TODO: [Оптимизация] Здесь, и в маппере, имеется огромный простор для оптимизации.
    Проверить, строится ли при одинаковой проекции и загруженных в контекст элементах,
    необходимых для проекции, каждый раз новый запрос к БД (вместо использования Local кеша).
     */
    public abstract class EFGenericRepository<TDomain, TEntity> : IRepository<TDomain>
        where TEntity : BaseEntity where TDomain : BaseDomain
    {
        protected DbContext dbContext;
        protected DbSet<TEntity> entities { get; set; }

        // TODO: Поле создано для инклюда всех связанных сущностей. Переосмыслить выборку данных из БД,
        // вместе с существованием этого поля.

        /// <summary>
        /// TODO: Поле создано для инклюда всех связанных сущностей. Переосмыслить выборку данных из БД,
        /// вместе с существованием этого поля.
        /// </summary>
        protected abstract List<TEntity> listOfEntities { get; }
        protected IEnumerable<TDomain> domainModels => listOfEntities.Select(EntityToModel);

        private readonly ConditionalWeakTable<TEntity, TDomain> cache;

        public EFGenericRepository(DbContext dbContext)
        {
            cache = [];
            this.dbContext = dbContext;
            entities = dbContext.Set<TEntity>();
        }
        public List<TDomain> Where(Func<TDomain, bool> predicate)
        {
            return domainModels.Where(predicate).ToList();
        }

        public List<TDomain> GetAll()
        {
            return domainModels.ToList();
        }

        public TDomain? SingleOrDefault(Func<TDomain, bool> predicate)
        {
            return domainModels.SingleOrDefault(predicate);
        }

        public TDomain? GetModel(int id)
        {
            return domainModels.SingleOrDefault(e => e.Id == id);
        }

        public bool Any(Func<TDomain, bool> predicate)
        {
            return domainModels.Any(predicate);
        }

        public void Create(TDomain model)
        {
            var entity = ModelToEntity(model);
            entities.Add(entity);
            SaveChanges();
        }
        public bool Update(TDomain model)
        {
            var e = GetEntity(model.Id);
            if (e != null && domainModels.Contains(model))
            {
                e = ModelToEntity(model);
                entities.Update(e);
                SaveChanges();
                return true;
            }
            else
                return false;
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
        protected TEntity? GetEntity(int id)
        {
            return listOfEntities.SingleOrDefault(e => e.Id == id);
        }

        /// <summary>
        /// Маппинг entity в model.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected abstract TDomain EntityToModel(TEntity entity);

        /// <summary>
        /// Маппинг model в entity.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected abstract TEntity ModelToEntity(TDomain model);


    }
}
