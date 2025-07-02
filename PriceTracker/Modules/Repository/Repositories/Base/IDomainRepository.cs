using PriceTracker.Core.Models.Domain;

namespace PriceTracker.Modules.Repository.Repositories.Base
{
    public interface IDomainRepository<TCoreDto> : IRepository<TCoreDto>
        where TCoreDto : DomainDto
    {

        /// <summary>
        /// Возвращается один элемент репозитория с указанным Id, либо, если такого элемента нет, <c>default(TEntity)</c>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если в последовательности имеется 2 поля с одинаковым Id.</exception>
        public TCoreDto? GetModel(int id)
        {
            return SingleOrDefault(m => m.Id == id);
        }


    }
}
