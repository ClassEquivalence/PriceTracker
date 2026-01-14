using PriceTracker.Core.Models.Process;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ParsingLayer.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.UpsertionLayer;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.UpsertionInvocationLayer
{

    public abstract class ScheduledMerchUpserter
    {
        public abstract Task ProcessUpsertion();
        public virtual async Task OnShutDownAsync() { }
    }

    public abstract class ScheduledMerchUpserter<MerchParsedDto, ExecutionState> :
        ScheduledMerchUpserter
        where ExecutionState : ExtractionStateDto
        where MerchParsedDto : MerchParsingDto
    {
        protected readonly IMerchUpsertionProcessor MerchUpserter;

        protected readonly ILogger? _logger;

        private readonly TimeSpan _upsertionRestPeriod;
        private readonly TimeSpan _upsertionCyclePeriod;
        private DateTime _upsertionStartTime;

        public event Action? UpsertionCycleCompleted;

        public ScheduledMerchUpserter(IMerchUpsertionProcessor merchUpserter,
            TimeSpan upsertionCyclePeriod, TimeSpan upsertionRestPeriod,
            DateTime upsertionStartTime, ILogger? logger = null)
        {
            MerchUpserter = merchUpserter;

            _upsertionCyclePeriod = upsertionCyclePeriod;
            _upsertionRestPeriod = upsertionRestPeriod;
            _upsertionStartTime = upsertionStartTime;

            _logger = logger;
        }

        public override async Task ProcessUpsertion()
        {
            while (true)
            {

                while (DateTime.Now < _upsertionStartTime)
                {
                    var upsertionContinueTimeSpan = _upsertionStartTime - DateTime.Now;

                    _logger?.LogInformation($"{nameof(ScheduledMerchUpserter)}, {nameof(ProcessUpsertion)}: " +
                        $"Апсершн возобновится через {upsertionContinueTimeSpan.Days} дней," +
                        $" {upsertionContinueTimeSpan.Hours} часов и {upsertionContinueTimeSpan.Minutes} минут.");

                    await Task.Delay(_upsertionStartTime - DateTime.Now);
                }

                var result = await MerchUpserter.ProcessUpsertionSubCycleAsync();

                switch (result)
                {
                    case IMerchUpsertionProcessor.UpsertionSubcycleResult.SubcycleCompleted:
                        _upsertionStartTime = DateTime.Now + _upsertionRestPeriod;
                        continue;
                    case IMerchUpsertionProcessor.UpsertionSubcycleResult.EntireCycleCompleted:
                        _upsertionStartTime = DateTime.Now + _upsertionCyclePeriod;
                        continue;
                }
            }
        }

    }
}
