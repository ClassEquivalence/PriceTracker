using PriceTracker.Core.Models.Process;

namespace PriceTracker.Modules.Repository.Facade.FacadeInterfaces
{
    public interface IExtractionExecutionStateProvider
        <ExtractionStateInfo> where ExtractionStateInfo : ExtractionStateDto
    {
        /// <summary>
        /// Предоставляет экземпляр состояния извлечения данных.
        /// ВАЖНО: Такой экземпляр внутри БД должен существовать всего 1 на
        /// 1 регулярный процесс.
        /// </summary>
        /// <returns></returns>
        public ExtractionStateInfo Provide();

        public void Save(ExtractionStateInfo info);
    }
}
