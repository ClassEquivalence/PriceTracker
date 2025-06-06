using Microsoft.EntityFrameworkCore;
using PriceTracker.Models.DataAccess.Entities;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Modules.Repository.DataAccess.Repositories
{
    public interface IRepository<TDomain> where TDomain : BaseDomain
    {

        /// <summary>
        /// Метод возвращает список элементов репозитория, удовлетворяющих заданному условию.
        /// </summary>
        /// <param name="predicate">Условие выбора элементов репозитория</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Выбрасывается, если входной аргумент имеет значение null</exception>
        public List<TDomain> Where(Func<TDomain, bool> predicate);

        /// <summary>
        /// Метод возвращает все элементы репозитория.
        /// </summary>
        /// <returns></returns>
        public List<TDomain> GetAll()
        {
            return Where(m => true);
        }

        /// <summary>
        /// Возвращается один элемент репозитория, удовлетворяющий условию, либо, если таких элементов нет, <c>default(TEntity)</c>.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Выбрасывается, если входной аргумент имеет значение null</exception>
        /// <exception cref="InvalidOperationException">Выбрасывается, если условию удовлетворяет более 1-го элемента.</exception>
        public TDomain? SingleOrDefault(Func<TDomain, bool> predicate);

        /// <summary>
        /// Возвращается один элемент репозитория с указанным Id, либо, если такого элемента нет, <c>default(TEntity)</c>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если в последовательности имеется 2 поля с одинаковым Id.</exception>
        public TDomain? GetModel(int id)
        {
            return SingleOrDefault(m => m.Id == id);
        }

        public void Create(TDomain entity);

        /// <summary>
        /// Возвращается true, если приведение элемента к обновленному состоянию прошло успешно, false - если неуспешно
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если в последовательности имеется >=2 объектов с указанным Id.</exception>
        public bool Update(TDomain entity);

        /// <summary>
        /// Возвращается true, если удаление элемента прошло успешно, false - если элемент не был удален.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если в последовательности имеется не менее 2 объектов с одинаковым Id.</exception>
        public bool Delete(int id);
        public void SaveChanges();

        public bool Any(Func<TDomain, bool> predicate);
    }
}
