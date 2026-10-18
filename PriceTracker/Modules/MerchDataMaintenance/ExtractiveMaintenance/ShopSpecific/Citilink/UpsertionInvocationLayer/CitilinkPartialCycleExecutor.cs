using PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer
{

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

        public enum PartialCycleStopReason
        {
            NoUnprocessedNodesRemain, RequestQuotaExhausted, UnallowedResponse
        }

        public async Task<PartialCycleStopReason> ExecutePartialCycleAsync(int requestsLimitPerCycle)
        {
            _logger?.LogInformation($"Partial cycle's starting now");
            RequestsQuota quota = new(requestsLimitPerCycle);

            var anyUnprocessedNode = await _treeRepository.GetUnprocessedNodeAsync();
            if (anyUnprocessedNode is null)
            {
                await _treeRepository.RecreateTreeAsync();
            }
            anyUnprocessedNode = await _treeRepository.GetUnprocessedNodeAsync();
            if (anyUnprocessedNode is null)
                throw new InvalidOperationException();

            while (quota.RequestsRemainingCount > 0)
            {
                var unprocessedNode = await _treeRepository.GetUnprocessedNodeAsync();

                if (unprocessedNode == null)
                    return PartialCycleStopReason.NoUnprocessedNodesRemain;

                var nodeProcessingResult = await _nodeProcessor.
                    ProcessNodeAsync(unprocessedNode, quota);
                switch (nodeProcessingResult)
                {
                    case NodeProcessingStatus.Processed:
                        break;
                    case NodeProcessingStatus.RequestsQuotaExhausted:
                        return PartialCycleStopReason.RequestQuotaExhausted;
                        break;
                    case NodeProcessingStatus.UnexpectedResponse:
                        return PartialCycleStopReason.UnallowedResponse;
                        break;
                    case not NodeProcessingStatus.Processed:
                        throw new InvalidOperationException();
                        break;
                }
            }

            return PartialCycleStopReason.RequestQuotaExhausted;
        }
    }
}
