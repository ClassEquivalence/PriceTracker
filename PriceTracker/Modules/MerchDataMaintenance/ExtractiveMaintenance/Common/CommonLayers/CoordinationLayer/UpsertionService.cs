using PriceTracker.Core.HelperUtils;
using PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.CoordinationLayer
{
    public class UpsertionService
    {
        private readonly FullCycleService _citilinkExecutor;
        private readonly ICitilinkCycleControlRepositoryAdapter _cycleControlRepository;
        public UpsertionService(FullCycleService citilinkExecutor,
            ICitilinkCycleControlRepositoryAdapter cycleControlRepository)
        {
            _citilinkExecutor = citilinkExecutor;
            _cycleControlRepository = cycleControlRepository;
        }

        public async Task ProcessUpsertion()
        {
            bool cycleFinished = false;
            while (true)
            {
                FullCycleStopReason stopReason;
                if (cycleFinished)
                {
                    var allowedTime = await _cycleControlRepository.GetAllowedCycleStartTime();
                    await WaitUntilTime.WaitUntil(allowedTime.fullAvailableAt);
                    stopReason = await _citilinkExecutor.StartFullCycleAnewAsync();
                }
                else
                {
                    stopReason = await _citilinkExecutor.ContinueFullCycleAsync();
                }
                switch (stopReason)
                {
                    case FullCycleStopReason.SuccesfullyCompleted:
                        break;

                    case FullCycleStopReason.UnhandleableError:
                        throw new InvalidOperationException();
                }
            }
        }

        public async Task StopProcessingUpsertion()
        {
            throw new NotImplementedException();
        }

    }
}
