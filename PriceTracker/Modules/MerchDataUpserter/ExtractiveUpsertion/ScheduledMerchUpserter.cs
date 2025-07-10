using PriceTracker.Modules.MerchDataUpserter.Core;
using PriceTracker.Modules.MerchDataUpserter.Core.Models.ForParsing;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion
{

    public abstract class ScheduledMerchUpserter : IMerchUpserter
    {
        public abstract Task ProcessUpsertion();
    }

    public class ScheduledMerchUpserter<MerchParsedDto, ExecutionState> :
        ScheduledMerchUpserter
        where ExecutionState : ExtractionState
        where MerchParsedDto : MerchParsingDto
    {
        private readonly IMerchDataConsumer<MerchParsedDto> _dataConsumer;
        private readonly IMerchDataExtractor<MerchParsedDto, ExecutionState>
            _dataExtractor;

        private readonly TimeSpan _upsertionCyclePeriod;
        private DateTime _upsertionStartTime;
        private readonly ExecutionState _executionState;

        public ScheduledMerchUpserter(IMerchDataConsumer<MerchParsedDto> dataConsumer,
            IMerchDataExtractor<MerchParsedDto, ExecutionState> dataExtractor,
            TimeSpan upsertionCyclePeriod,
            DateTime upsertionStartTime, ExecutionState executionState)
        {
            _dataConsumer = dataConsumer;
            _dataExtractor = dataExtractor;
            _upsertionCyclePeriod = upsertionCyclePeriod;
            _upsertionStartTime = upsertionStartTime;
            _executionState = executionState;
        }

        public override async Task ProcessUpsertion()
        {
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

                _upsertionStartTime = DateTime.Now + _upsertionCyclePeriod;
                await task;


            }
        }

    }
}
