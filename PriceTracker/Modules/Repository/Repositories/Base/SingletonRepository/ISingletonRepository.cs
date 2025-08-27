using PriceTracker.Core.Models;

namespace PriceTracker.Modules.Repository.Repositories.Base.SingletonRepository
{
    public interface ISingletonRepository<Dto>
        where Dto: BaseDto
    {
        /// <summary>
        /// Вернуть null, если такого объекта в БД нет.
        /// </summary>
        /// <returns></returns>
        Dto? Get();
        void Set(Dto dto);
    }
}
