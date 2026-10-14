using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models
{
    public enum CitilinkMerchNodeScrapeTargetsAugmentationState
    {
        NotAugmented,
        AtLeastOneJsonTargetPresent,
        AllTargetsPresent
    }

    public enum CitilinkNodeType { MerchCatalog, RootCatalog, CatalogsCatalog, Unknown }
    public class CitilinkNode
    {
        public CitilinkScrapeTarget DefaultLocation;
        private List<CitilinkScrapeTarget> locationsList;
        public IReadOnlyList<CitilinkScrapeTarget> LocationsList => locationsList.
            Append(DefaultLocation).ToList();
        public CitilinkNodeType NodeType;
        public bool ScrapeTargetsProcessed;

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
            CitilinkNodeType type = CitilinkNodeType.Unknown,
            bool scrapeTargetsProcessed = false,
            int processingErrorCount = 0)
        {
            if (defaultLocation.Purpose != CitilinkScrapeTargetPurpose.Default)
            {
                throw new ArgumentException($"Должна была передаться локация с дефолтным контентом. {defaultLocation.Purpose}");
            }
            DefaultLocation = defaultLocation;
            NodeType = type;
            locationsList = [];
            ScrapeTargetsProcessed = scrapeTargetsProcessed;
            ProcessingErrorCount = processingErrorCount;
        }
        public void AddWebLocation(CitilinkScrapeTarget webLocation)
        {
            locationsList.Add(webLocation);
        }

        public override string ToString()
        {
            return $"Node: url {DefaultLocation.Url}, NodeType {NodeType}";
        }
    }
}
