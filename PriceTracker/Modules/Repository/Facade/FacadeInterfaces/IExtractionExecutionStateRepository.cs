using PriceTracker.Core.Models.Process;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;

namespace PriceTracker.Modules.Repository.Facade.FacadeInterfaces
{
    public interface IExtractionExecutionStateRepository
        <ExtractionStateInfo> where ExtractionStateInfo : ExtractionStateDto
    {
        /// <summary>
        /// Предоставляет экземпляр состояния извлечения данных.
        /// ВАЖНО: Такой экземпляр внутри БД должен существовать всего 1 на
        /// 1 регулярный процесс.
        /// <br/>
        /// Вернуть null, если объекта пока нет в БД.
        /// </summary>
        /// <returns></returns>
        public ExtractionStateInfo? Provide();

        public void Save(ExtractionStateInfo info);

        public void UpdateBranch(CitilinkBranch branch);
    }
}
