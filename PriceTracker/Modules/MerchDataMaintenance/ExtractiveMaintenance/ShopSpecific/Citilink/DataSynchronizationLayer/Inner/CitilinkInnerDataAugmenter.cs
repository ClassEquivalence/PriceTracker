using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Inner.CatalogTreeMaintainer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Upsertion;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Inner
{
    public class CitilinkInnerDataAugmenter
    {
        private readonly IInnerCitilinkMerchUpserter _merchUpserter;
        private readonly ICitilinkCatalogTreeMaintainer _treeMaintainer;
        private readonly ILogger? _logger;

        private const int _perPage = 36;
        public CitilinkInnerDataAugmenter(IInnerCitilinkMerchUpserter merchUpserter,
            ICitilinkCatalogTreeMaintainer treeMaintainer, ILogger? logger = null)
        {
            _merchUpserter = merchUpserter;
            _treeMaintainer = treeMaintainer;
            _logger = logger;
        }


        public async Task CompleteAsync()
        {
            await _merchUpserter.CompleteAsync();
        }


        public async Task AugmentNodeAsync(CitilinkNode node, CitilinkParsingSnapshot nodeDataSnapshot)
        {
            _logger?.LogTrace($"{nameof(CitilinkInnerDataAugmenter)}, {nameof(AugmentNodeAsync)}:" +
                $" Node data's being augmented.");
            await TryAugmentWithType(node, nodeDataSnapshot);
            await TryAugmentWithDescendants(node, nodeDataSnapshot);
            await TryAugmentWithMerchesAsync(node, nodeDataSnapshot);
            await TryAugmentWithScrapeTargets(node, nodeDataSnapshot);
        }

        public async Task MarkNodeLocationAsProcessed(CitilinkNode node, CitilinkScrapeTarget
            nodeLocation)
        {
            if (!node.LocationsList.Contains(nodeLocation))
                throw new InvalidOperationException();
            nodeLocation.IsProcessed = true;
            await _treeMaintainer.UpdateNodeAsync(node);
        }

        private async Task TryAugmentWithType(CitilinkNode node, CitilinkParsingSnapshot snapshot)
        {
            node.NodeType = NodeTypeByParsableType(snapshot.Type ?? ParsableType.Unknown);
            if (node.NodeType != CitilinkNodeType.Unknown)
                node.AugmentationState = CitilinkNodeAugmentationState.PartiallyProcessed_TypeIsKnown;
            await _treeMaintainer.UpdateNodeAsync(node);
        }
        private async Task TryAugmentWithDescendants(CitilinkNode node, CitilinkParsingSnapshot snapshot)
        {
            List<string>? childrenUrls;
            if ((childrenUrls = snapshot.DescendantNodeUrls) != null && childrenUrls.Count > 0)
            {
                var webLocationDataList = childrenUrls.Select<string, CitilinkScrapeTarget>
                    (url => new CitilinkScrapeTarget(url)).ToList();
                await _treeMaintainer.FillDescendantsAsync(node, webLocationDataList);
            }
        }
        private async Task TryAugmentWithMerchesAsync(CitilinkNode node, CitilinkParsingSnapshot snapshot)
        {
            List<CitilinkMerchParsingDto>? merches;
            if ((merches = snapshot.Merches) != null && merches.Count > 0)
            {
                _logger?.LogTrace($"Merches of {merches.Count} are augmented");
                foreach (var merchDto in merches)
                {
                    await _merchUpserter.EnqueueForUpsertionAsync(merchDto);
                }

            }
        }

        // Try Augment with... scrape targets/web locations?
        private async Task TryAugmentWithScrapeTargets(CitilinkNode node, CitilinkParsingSnapshot snapshot)
        {
            // Проверить не добавлена ли локация уже, затем добавить.
            // Если парсер должен знать о структуре сайта то почему добавлением
            // занимается датасинк-слой?

            switch (node.NodeType)
            {
                case CitilinkNodeType.MerchCatalog:
                    TryAugmentMerchNodeWithScrapeTargets(node, snapshot);
                    await _treeMaintainer.UpdateNodeAsync(node);
                    break;
                default:
                    break;
            }
        }

        private void TryAugmentMerchNodeWithScrapeTargets(CitilinkNode node, 
            CitilinkParsingSnapshot snapshot)
        {
            if (node.NodeType != CitilinkNodeType.MerchCatalog)
                throw new ArgumentException();

            switch (node.MerchNodeScrapeTargetsAugmentationState)
            {
                case CitilinkMerchNodeScrapeTargetsAugmentationState.NotAugmented:
                    if (snapshot.NodeMerchesCount != null && snapshot.NodeMerchesCount > 0)
                    {
                        FullyAugment();
                        node.MerchNodeScrapeTargetsAugmentationState = 
                            CitilinkMerchNodeScrapeTargetsAugmentationState.AllTargetsPresent;
                    }
                    else
                    {
                        AugmentOneJson();
                        node.MerchNodeScrapeTargetsAugmentationState =
                            CitilinkMerchNodeScrapeTargetsAugmentationState.OneJsonTargetPresent;
                    }
                    break;
                case CitilinkMerchNodeScrapeTargetsAugmentationState.OneJsonTargetPresent:
                    if (snapshot.NodeMerchesCount != null && snapshot.NodeMerchesCount > 0)
                    {
                        FullyAugment();
                        node.MerchNodeScrapeTargetsAugmentationState =
                            CitilinkMerchNodeScrapeTargetsAugmentationState.AllTargetsPresent;
                    }
                    break;
                case CitilinkMerchNodeScrapeTargetsAugmentationState.AllTargetsPresent:
                    break;
            }

            void FullyAugment()
            {
                var mainTarget = node.DefaultLocation;
                string categorySlug = node.CategorySlug;


                var merchesCount = Math.Ceiling((double)snapshot.NodeMerchesCount / _perPage);

                _logger?.LogTrace($"{nameof(TryAugmentMerchNodeWithScrapeTargets)}, " +
                    $"{nameof(FullyAugment)}: nodeMerchesCount is {merchesCount} ");

                for (int i = 1; i < merchesCount; i++)
                {
                    CitilinkScrapeTarget target = new CitilinkScrapeMerchJsonTarget
                        (categorySlug, page: i, perPage: _perPage);

                    if (!node.LocationsList.Any
                        (t => t is CitilinkScrapeMerchJsonTarget && 
                        (t as CitilinkScrapeMerchJsonTarget).Page == i))
                    {
                        node.AddWebLocation(target);
                    }
                }
            }

            void AugmentOneJson()
            {
                var mainTarget = node.DefaultLocation;
                string categorySlug = node.CategorySlug;
                CitilinkScrapeTarget target = new CitilinkScrapeMerchJsonTarget
                        (categorySlug, page: 1, perPage: _perPage);
                node.AddWebLocation(target);
            }
        }

        private CitilinkNodeType NodeTypeByParsableType(ParsableType parsableType)
        {
            return parsableType switch
            {
                ParsableType.MerchCatalog => CitilinkNodeType.MerchCatalog,
                ParsableType.CatalogsCatalog => CitilinkNodeType.CatalogsCatalog,
                ParsableType.MerchesJson => CitilinkNodeType.MerchCatalog,
                ParsableType.RootCatalog => CitilinkNodeType.RootCatalog,
                ParsableType.Unknown => CitilinkNodeType.Unknown,
                _ => throw new InvalidOperationException()
            };
        }
    }
}
