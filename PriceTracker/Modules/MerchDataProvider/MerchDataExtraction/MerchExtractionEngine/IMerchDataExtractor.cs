using PriceTracker.Modules.MerchDataProvider.MerchDataExtraction.ExecutionState;
using PriceTracker.Modules.MerchDataProvider.Models.ForParsing;

namespace PriceTracker.Modules.MerchDataProvider.MerchDataExtraction.MerchExtractionEngine
{

    /// <summary>
    /// Не стоит реализовывать этот интерфейс. Стоит реализовывать обобщенный вариант.
    /// </summary>
    public interface IMerchDataExtractor { }

    public interface IMerchDataExtractor<Dto, ExecutionState> : IMerchDataExtractor
        where Dto : MerchParsingDto where ExecutionState : ExtractionExecutionStateInfo
    {
        /// <summary>
        /// Если передана extractionData - процесс продолжается с момента в ней.
        /// Если не передана - процесс начинается с нуля.
        /// </summary>
        /// <param name="extractionData"></param>
        /// <returns></returns>
        IAsyncEnumerable<Dto> RunExtractionProcess(ExecutionState? extractionData = null);
        ExecutionState? GetProgress();
        public event Action<ExecutionState>? OnExecutionStateUpdate;
    }
}
