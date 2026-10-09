using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models
{
    /// <summary>
    /// Processed = обработана и ветвь и рекурсивно все её потомки.
    /// TypeDiscovered = Уточнен тип ветви, в остальном она не обработана.
    /// DescendantsUnprocessed = Уточнен тип ветви, но наследники обработаны как минимум не полностью.
    /// TotallyUnprocessed = обработка ветви не начиналась.
    /// 
    /// </summary>
    public enum CitilinkNodeAugmentationState
    {
        Processed,
        PartiallyProcessed_TypeIsKnown,
        PartiallyProcessed_TypeIsUnknown,
        Unprocessed
    }

    public enum CitilinkMerchNodeScrapeTargetsAugmentationState
    {
        NotAugmented,
        OneJsonTargetPresent,
        AllTargetsPresent
    }

    public enum CitilinkNodeType { MerchCatalog, RootCatalog, CatalogsCatalog, Unknown }
    public class CitilinkNode
    {
        public CitilinkScrapeTarget DefaultLocation;
        private List<CitilinkScrapeTarget> locationsList;
        public IReadOnlyList<CitilinkScrapeTarget> LocationsList => locationsList.
            Append(DefaultLocation).ToList();

        public CitilinkNodeAugmentationState AugmentationState;
        public CitilinkNodeType NodeType;
        public bool DescendantsProcessed;

        public int ProcessingErrorCount;

        public CitilinkMerchNodeScrapeTargetsAugmentationState
            MerchNodeScrapeTargetsAugmentationState =
            CitilinkMerchNodeScrapeTargetsAugmentationState.NotAugmented;

        public string CategorySlug
        {
            get
            {
                return DefaultLocation.Url.Replace("https://www.citilink.ru/catalog/", "")
                .Split('/')[0].Split("--")[0];
            }
        }

        public CitilinkNode(CitilinkScrapeTarget defaultLocation,
            CitilinkNodeAugmentationState state, CitilinkNodeType type = CitilinkNodeType.Unknown,
            bool descendantsProcessed = false,
            int processingErrorCount = 0)
        {
            if (defaultLocation.Purpose != CitilinkScrapeTargetPurpose.Default)
            {
                throw new ArgumentException($"Должна была передаться локация с дефолтным контентом. {defaultLocation.Purpose}");
            }
            DefaultLocation = defaultLocation;
            AugmentationState = state;
            NodeType = type;
            locationsList = [];
            DescendantsProcessed = descendantsProcessed;
            ProcessingErrorCount = processingErrorCount;
        }
        public void AddWebLocation(CitilinkScrapeTarget webLocation)
        {
            locationsList.Add(webLocation);
        }

        public override string ToString()
        {
            return $"Node: url {DefaultLocation.Url}, AugmentationState {AugmentationState}, " +
                $"NodeType {NodeType}";
        }
    }
}
