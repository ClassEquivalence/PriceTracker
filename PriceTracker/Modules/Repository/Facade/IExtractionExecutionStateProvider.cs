using PriceTracker.Modules.MerchDataProvider.MerchDataExtraction.ExecutionState;

namespace PriceTracker.Modules.Repository.Facade
{
    public interface IExtractionExecutionStateProvider
        <ExtractionStateInfo> where ExtractionStateInfo : ExtractionExecutionStateInfo
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
