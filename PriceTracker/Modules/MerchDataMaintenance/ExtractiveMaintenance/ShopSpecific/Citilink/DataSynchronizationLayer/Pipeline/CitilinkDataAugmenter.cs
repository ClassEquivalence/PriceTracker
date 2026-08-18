using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Pipeline
{
    public class CitilinkDataAugmenter : ICitilinkDataAugmenter
    {
        private readonly ICitilinkParser _parser;
        private readonly CitilinkInnerDataAugmenter _innerAugmenter;
        private readonly ILogger? _logger;
        public CitilinkDataAugmenter(CitilinkInnerDataAugmenter innerDataAugmenter,
            ICitilinkParser parser, ILogger logger)
        {
            _innerAugmenter = innerDataAugmenter;
            _parser = parser;
            _logger = logger;
        }

        public async Task CompleteAsync()
        {
            await _innerAugmenter.CompleteAsync();
        }

        public async Task<CitilinkExecutionMetadata>
            AugmentNodeDataAsync(CitilinkNode node, ICitilinkScraperPoliciesEnforcer enforcer)
        {
            var nodeLocation = GetUnprocessed(node);
            if (nodeLocation == null)
            {
                CitilinkExecutionMetadata metadata = new();
                metadata.DataSyncMetadata = new(CitilinkNodeAugmenterAugmentationState.
                    NoUnprocessedLocationsLeft);
                return metadata;
            }
            var parseResult = await _parser.ParseUnitAsync(nodeLocation, enforcer);

            var pipelineMetadata = parseResult.Info;
            if (pipelineMetadata.ParseMetadata?.State == ParseCompletionState.Success)
            {
                CitilinkParsingSnapshot? snapshot;
                if ((snapshot = parseResult.Result) == null)
                    throw new NullReferenceException();
                await _innerAugmenter.AugmentNodeAsync(node, snapshot);
                pipelineMetadata.DataSyncMetadata = new(CitilinkNodeAugmenterAugmentationState.Success);
                await _innerAugmenter.MarkNodeLocationAsProcessed(node, nodeLocation);
                return pipelineMetadata;
            }
            else
            {
                pipelineMetadata.DataSyncMetadata = new(CitilinkNodeAugmenterAugmentationState.Failure_PreviousLayer);
                return pipelineMetadata;
            }
        }

        private CitilinkScrapeTarget? GetUnprocessed(CitilinkNode node)
        {
            var nodeLocation = node.DefaultLocation;
            if (!nodeLocation.IsProcessed)
                return nodeLocation;
            else
            {
                return node.LocationsList.FirstOrDefault
                    (nodeLocation => !nodeLocation.IsProcessed);
            }
        }
    }
}

