using PriceTracker.Modules.Repository.Entities;

namespace PriceTracker.Modules.Repository.Repositories.Base
{
    public interface IEntityRepository<TEntity>
        where TEntity : BaseEntity
    {
        public List<TEntity> Where(Func<TEntity, bool>? predicate = null);
        public bool Any(Func<TEntity, bool>? predicate = null);

        public void Create(TEntity entity);
        public bool Update(TEntity entity);
        public bool Delete(int id);


        /// <summary>
        /// Выбирается Entity с указанным id. Возвращает null, если элемент с указаным id не найден.
        /// Выбрасывает исключение, если в последовательности имеется не менее 2 объектов с одинаковым Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public TEntity? GetEntity(int id);
    }
}
