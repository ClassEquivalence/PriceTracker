using PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer
{
    public enum PartialCycleStopReason
    {
        NoUnprocessedNodesRemain, RequestQuotaExhausted, UnallowedResponse
    }

    public class CitilinkPartialCycleExecutor
    {
        private readonly ILogger? _logger;
        private readonly ICitilinkCatalogTreeRepositoryAdapter _treeRepository;

        private readonly CitilinkNodeProcessor _nodeProcessor;

        public CitilinkPartialCycleExecutor(
            ICitilinkCatalogTreeRepositoryAdapter catalogTreeRepository,
            CitilinkNodeProcessor nodeProcessor, ILogger? logger)
        {
            _treeRepository = catalogTreeRepository;
            _nodeProcessor = nodeProcessor;
            _logger = logger;
        }

        public async Task<(PartialCycleStopReason stopReason, int requestsSpent)> 
            ExecutePartialCycleAsync(int requestsLimitPerCycle)
        {
            _logger?.LogInformation($"Partial cycle's starting now");
            RequestsQuota quota = new(requestsLimitPerCycle);

            while (quota.RequestsRemainingCount > 0)
            {
                var unprocessedNode = await _treeRepository.GetUnprocessedNodeAsync();

                if (unprocessedNode == null)
                    return (PartialCycleStopReason.NoUnprocessedNodesRemain, 
                        quota.RequestsRemainingCount-requestsLimitPerCycle);

                var nodeProcessingResult = await _nodeProcessor.
                    ProcessNodeAsync(unprocessedNode, quota);
                switch (nodeProcessingResult)
                {
                    case NodeProcessingStatus.Processed:
                        break;
                    case NodeProcessingStatus.RequestsQuotaExhausted:
                        return (PartialCycleStopReason.RequestQuotaExhausted,
                        quota.RequestsRemainingCount - requestsLimitPerCycle);
                        break;
                    case NodeProcessingStatus.UnexpectedResponse:
                        return (PartialCycleStopReason.UnallowedResponse,
                        quota.RequestsRemainingCount - requestsLimitPerCycle);
                        break;
                    case not NodeProcessingStatus.Processed:
                        throw new InvalidOperationException();
                        break;
                }
            }
            return (PartialCycleStopReason.RequestQuotaExhausted, 
                quota.RequestsRemainingCount - requestsLimitPerCycle);
        }
    }
}
