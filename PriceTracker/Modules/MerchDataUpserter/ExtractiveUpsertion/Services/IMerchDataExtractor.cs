using PriceTracker.Core.Models.Process;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ForParsing;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services
{


    public enum ExtractionPartialCycleResult
    {
        HaltedAsFinished,
        HaltedAsTired,
        CycleNotFinished,
        UnknownError,
        HaltIssued
    }

    /// <summary>
    /// Не стоит реализовывать этот интерфейс. Стоит реализовывать обобщенный вариант.
    /// </summary>
    public interface IMerchDataExtractor { }

    public interface IMerchDataExtractor<Dto, ExecutionState> : IMerchDataExtractor
        where Dto : MerchParsingDto where ExecutionState : ExtractionStateDto
    {



        /// <summary>
        /// HaltedAsFinished - остановлено, т.к. полный цикл апсершна завершился.
        /// <br/>
        /// HaltedAsTired - остановлено, т.к. завершился частичный, но не полный 
        /// цикл апсершна.
        /// <br/>
        /// CycleNotFinished - частичный цикл извлечения ещё не завершен/даже не начинался.
        /// </summary>
        /// <returns></returns>
        public ExtractionPartialCycleResult GetResult();

        /// <summary>
        /// Начать процесс извлечения товаров.
        /// Процесс извлечения может быть приостановлен вместе с завершением метода.
        /// Метод также завершает работу при окончании цикла апсершна.
        /// </summary>
        /// <returns></returns>
        IAsyncEnumerable<Dto> RunExtractionProcess();

        /// <summary>
        /// Возобновить процесс извлечения товаров.
        /// Процесс извлечения может быть приостановлен вместе с завершением метода.
        /// Метод также завершает работу при окончании цикла апсершна.
        /// </summary>
        /// <returns></returns>
        IAsyncEnumerable<Dto> ContinueExtractionProcess(ExecutionState extractionData);
        ExecutionState? GetProgress();
        public event Action<ExecutionState>? OnExecutionStateUpdate;
        public event Action ExtractionProcessFinished;
    }
}
