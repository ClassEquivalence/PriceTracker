using PriceTracker.Core.HelperUtils;
using PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.CoordinationLayer
{
    public enum FullCycleStopReason
    {
        SuccesfullyCompleted, UnhandleableError
    }
    public class FullCycleService
    {
        private readonly CitilinkPartialCycleExecutor _citilinkExecutor;
        private readonly ICitilinkCycleControlRepositoryAdapter _cycleControlRepository;
        private readonly ICitilinkCatalogTreeRepositoryAdapter _catalogTreeRepository;
        public FullCycleService(CitilinkPartialCycleExecutor citilinkExecutor,
            ICitilinkCycleControlRepositoryAdapter cycleControlRepository,
            ICitilinkCatalogTreeRepositoryAdapter catalogTreeRepository)
        {
            _citilinkExecutor = citilinkExecutor;
            _cycleControlRepository = cycleControlRepository;
            _catalogTreeRepository = catalogTreeRepository;
        }
        public async Task<FullCycleStopReason> StartFullCycleAnewAsync(CancellationToken cancellationToken)
        {
            await _catalogTreeRepository.RecreateTreeAsync();
            return await ProcessFullCycle(cancellationToken);
        }

        public async Task<FullCycleStopReason> ContinueFullCycleAsync(CancellationToken cancellationToken)
        {
            return await ProcessFullCycle(cancellationToken);
        }

        private async Task<PartialCycleStopReason?> TryRunPartialCycleAsync(CancellationToken cancellationToken)
        {
            var quota = await _cycleControlRepository.TryGetPartialCycleRequestsQuotaAsync();
            if(quota <= 0)
            {
                return null;
            }
            else
            {
                var cycleResult = await _citilinkExecutor.ExecutePartialCycleAsync(quota);
                await _cycleControlRepository.RegisterSpentRequestsAsync(cycleResult.requestsSpent);
                return cycleResult.stopReason;
            }
        }

        private async Task<FullCycleStopReason> ProcessFullCycle(CancellationToken cancellationToken)
        {
            while (true)
            {
                var partialStopReason = await TryRunPartialCycleAsync(cancellationToken);
                switch (partialStopReason)
                {
                    case null or PartialCycleStopReason.RequestQuotaExhausted:
                        var (partialAvailableAt, _) =
                            await _cycleControlRepository.GetAllowedCycleStartTime();
                        await WaitUntilTime.WaitUntil(partialAvailableAt);
                        break;

                    case PartialCycleStopReason.NoUnprocessedNodesRemain:
                        return FullCycleStopReason.SuccesfullyCompleted;

                    case PartialCycleStopReason.UnallowedResponse:
                        return FullCycleStopReason.UnhandleableError;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}