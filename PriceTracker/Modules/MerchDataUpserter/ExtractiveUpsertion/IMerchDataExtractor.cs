using PriceTracker.Modules.MerchDataUpserter.Core.Models.ForParsing;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion
{

    /// <summary>
    /// Не стоит реализовывать этот интерфейс. Стоит реализовывать обобщенный вариант.
    /// </summary>
    public interface IMerchDataExtractor { }

    public interface IMerchDataExtractor<Dto, ExecutionState> : IMerchDataExtractor
        where Dto : MerchParsingDto where ExecutionState : ExtractionState
    {

        IAsyncEnumerable<Dto> RunExtractionProcess();

        IAsyncEnumerable<Dto> ContinueExtractionProcess(ExecutionState extractionData);
        ExecutionState? GetProgress();
        public event Action<ExecutionState>? OnExecutionStateUpdate;
        public event Action ExtractionProcessFinished;
    }
}
