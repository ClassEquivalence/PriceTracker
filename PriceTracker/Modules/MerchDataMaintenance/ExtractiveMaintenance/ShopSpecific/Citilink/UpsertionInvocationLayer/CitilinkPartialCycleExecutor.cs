using PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer
{

    public class CitilinkPartialCycleExecutor
    {
        private readonly ILogger? _logger;

        private readonly ICitilinkScraper _scraper;
        private readonly IInnerCitilinkParser _parser;
        private readonly CitilinkInnerDataAugmenter _dataAugmenter;
        private readonly ICitilinkCatalogTreeRepositoryAdapter _treeRepository;
        public CitilinkPartialCycleExecutor(ICitilinkScraper scraper,
            IInnerCitilinkParser parser, CitilinkInnerDataAugmenter dataAugmenter,
            ICitilinkCatalogTreeRepositoryAdapter catalogTreeRepository,
            ILogger? logger)
        {
            _scraper = scraper;
            _parser = parser;
            _dataAugmenter = dataAugmenter;
            _treeRepository = catalogTreeRepository;

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

                var nodeProcessingResult = await ProcessNodeAsync(unprocessedNode, quota);
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

        protected enum NodeProcessingStatus
        {
            Processed, RequestsQuotaExhausted, UnexpectedResponse
        }

        private async Task<NodeProcessingStatus>
            ProcessNodeAsync(CitilinkNode unprocessedNode, RequestsQuota quota)
        {
            _logger?.LogTrace($"Starting to process node {unprocessedNode.DefaultLocation.Url}");

            var unprocessedLocations = unprocessedNode.LocationsList.
                Where(l => !l.IsProcessed).ToList();
            while (unprocessedLocations.Count > 0)
            {
                var locationsProcessingResult = await ProcessLocationsToNodeAsync
                    (unprocessedNode, unprocessedNode.LocationsList.ToList(), quota);
                switch (locationsProcessingResult)
                {
                    case LocationsProcessingStatus.Processed:
                        break;

                    case LocationsProcessingStatus.RequestsQuotaExhausted:
                        _logger?.LogTrace($"Node {unprocessedNode.DefaultLocation.Url}" +
                            $" processing stopped: Requests quota's been exhausted.");
                        return NodeProcessingStatus.RequestsQuotaExhausted;
                        break;

                    case LocationsProcessingStatus.UnexpectedResponse:
                        _logger?.LogTrace($"Node {unprocessedNode.DefaultLocation.Url}" +
                            $" processing stopped: unexpected response's been received.");
                        return NodeProcessingStatus.UnexpectedResponse;
                        break;

                    case not LocationsProcessingStatus.Processed:
                        throw new InvalidOperationException();
                        break;
                }
                unprocessedLocations = unprocessedNode.LocationsList.
                Where(l => !l.IsProcessed).ToList();
            }

            await _treeRepository.UpdateNodeAsync(unprocessedNode);

            _logger?.LogTrace($"Node {unprocessedNode.DefaultLocation.Url}" +
                $" processing completed.");
            return NodeProcessingStatus.Processed;
        }


        protected enum LocationsProcessingStatus
        {
            Processed, RequestsQuotaExhausted, UnexpectedResponse
        }
        private async Task<LocationsProcessingStatus> ProcessLocationsToNodeAsync
            (CitilinkNode locationsOwnerNode, List<CitilinkScrapeTarget> unprocessedLocations,
            RequestsQuota quota)
        {
            foreach (var location in unprocessedLocations)
            {
                // Stage 1: scraping
                var scrapeResult = await _scraper.ScrapeAsync(location, quota);
                var scrapeState = scrapeResult.state;
                switch (scrapeState)
                {
                    case ScrapeState.RequestsQuotaExhausted:
                        return LocationsProcessingStatus.RequestsQuotaExhausted;
                        break;

                    case ScrapeState.UnallowedStatus:
                        return LocationsProcessingStatus.UnexpectedResponse;
                        break;

                    case not ScrapeState.Success:

                        break;
                }
                var scrapable = scrapeResult.scrapable ?? throw new InvalidOperationException();

                // Stage 2: parsing
                var parseResult = await _parser.ParseUnitAsync(scrapable);
                switch (parseResult.Info.State)
                {
                    case ParsingLayer.Models.ParseCompletionState.Failure_PreviousLayer:
                        // TODO: такого кейса в перспективе быть не должно т.к. идёт уход от
                        // слоистой модели.
                        throw new InvalidOperationException();
                        break;

                    case ParsingLayer.Models.ParseCompletionState.Unsuccess_UnknownParsable:
                        return LocationsProcessingStatus.UnexpectedResponse;
                        break;

                    case not ParsingLayer.Models.ParseCompletionState.Success:
                        break;
                }
                var parseSnapshot = parseResult.Result ?? throw new InvalidOperationException();

                // Stage 3: Data augmentation
                await _dataAugmenter.AugmentNodeAsync(locationsOwnerNode, parseSnapshot);
                await _dataAugmenter.MarkNodeLocationAsProcessed(locationsOwnerNode, location);
            }

            return LocationsProcessingStatus.Processed;
        }
    }
}
