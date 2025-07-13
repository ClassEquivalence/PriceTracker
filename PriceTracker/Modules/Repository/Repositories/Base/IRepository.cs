using PriceTracker.Core.Models;

namespace PriceTracker.Modules.Repository.Repositories.Base
{
    public interface IRepository<TCoreDto> where TCoreDto : BaseDto
    {
        /// <summary>
        /// Метод возвращает список элементов репозитория, удовлетворяющих заданному условию.
        /// </summary>
        /// <param name="predicate">Условие выбора элементов репозитория</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Выбрасывается, если входной аргумент имеет значение null</exception>
        public List<TCoreDto> Where(Func<TCoreDto, bool> predicate);

        /// <summary>
        /// Метод возвращает все элементы репозитория.
        /// </summary>
        /// <returns></returns>
        public List<TCoreDto> GetAll()
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
        public TCoreDto? SingleOrDefault(Func<TCoreDto, bool> predicate);

        public void Create(TCoreDto entity);

        /// <summary>
        /// Возвращается true, если приведение элемента к обновленному состоянию прошло успешно, false - если неуспешно
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если в последовательности имеется >=2 объектов с указанным Id.</exception>
        public bool Update(TCoreDto entity);

        /// <summary>
        /// Возвращается true, если удаление элемента прошло успешно, false - если элемент не был удален.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если в последовательности имеется не менее 2 объектов с одинаковым Id.</exception>
        public bool Delete(int id);

        public bool Any(Func<TCoreDto, bool> predicate);
    }
}
