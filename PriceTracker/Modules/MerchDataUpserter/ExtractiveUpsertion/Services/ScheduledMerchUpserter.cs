using PriceTracker.Core.Models.Process;
using PriceTracker.Modules.MerchDataUpserter.Core;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ForParsing;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services
{

    public abstract class ScheduledMerchUpserter : IMerchUpserter
    {
        public abstract Task ProcessUpsertion();
        public virtual async Task OnShutDownAsync() { }
    }

    public abstract class ScheduledMerchUpserter<MerchParsedDto, ExecutionState> :
        ScheduledMerchUpserter
        where ExecutionState : ExtractionStateDto
        where MerchParsedDto : MerchParsingDto
    {

        protected readonly ILogger? _logger;

        private readonly IMerchDataConsumer<MerchParsedDto> _dataConsumer;
        protected readonly IMerchDataExtractor<MerchParsedDto, ExecutionState>
            _dataExtractor;

        private readonly TimeSpan _upsertionRestPeriod;
        private readonly TimeSpan _upsertionCyclePeriod;
        private DateTime _upsertionStartTime;
        protected ExecutionState? _executionState;

        public event Action? UpsertionCycleCompleted;

        public ScheduledMerchUpserter(IMerchDataConsumer<MerchParsedDto> dataConsumer,
            IMerchDataExtractor<MerchParsedDto, ExecutionState> dataExtractor,
            TimeSpan upsertionCyclePeriod, TimeSpan upsertionRestPeriod,
            DateTime upsertionStartTime, ILogger? logger = null)
        {
            _dataConsumer = dataConsumer;
            _dataExtractor = dataExtractor;
            _upsertionCyclePeriod = upsertionCyclePeriod;
            _upsertionRestPeriod = upsertionRestPeriod;
            _upsertionStartTime = upsertionStartTime;

            _logger = logger;
        }

        public override async Task ProcessUpsertion()
        {
            if(_executionState == null)
            {
                _executionState = GetOrCreateExecState();
            }
            while (true)
            {

                while (DateTime.Now < _upsertionStartTime)
                {
                    await Task.Delay(_upsertionStartTime - DateTime.Now);
                }

                Task task;
                if (_executionState.IsCompleted)
                {
                    
                    task = _dataConsumer.Upsert(_dataExtractor.
                        RunExtractionProcess());

                }
                else
                {
                    task = _dataConsumer.Upsert(_dataExtractor.
                        ContinueExtractionProcess(_executionState));
                }


                if (!_executionState.IsCompleted)
                {
                    _upsertionStartTime = DateTime.Now + _upsertionRestPeriod;
                    await task;
                    return;
                }

                _upsertionStartTime = DateTime.Now + _upsertionCyclePeriod;

                //try
                //{
                await task;
                //}
                //catch (Exception ex)
                //{
                //_logger?.LogError($"{nameof(ScheduledMerchUpserter)}: {ex.Message}");
                //_logger?.LogError($"{nameof(ScheduledMerchUpserter)}(innerEx): {ex?.InnerException?.Message}");
                //}


            }
        }

        public ExecutionState GetOrCreateExecState()
        {
            return TryLoadExecutionState() ?? CreateNewExecutionState();
        }

        public abstract ExecutionState? TryLoadExecutionState();
        public abstract ExecutionState CreateNewExecutionState();

    }
}
