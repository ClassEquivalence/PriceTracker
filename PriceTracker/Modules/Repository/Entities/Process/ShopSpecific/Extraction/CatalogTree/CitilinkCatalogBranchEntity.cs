using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models;

namespace PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction.CatalogTree
{
    public class CitilinkCatalogBranchEntity : BaseEntity
    {
        public CitilinkCatalogBranchEntity(string url,
            bool scrapeTargetsProcessed, CitilinkNodeType nodeType, int id = default,
            int processingErrorCount = 0)
            : base(id)
        {
            NodeType = nodeType;
            Url = url;
            ScrapeTargetsProcessed = scrapeTargetsProcessed;
            ProcessingErrorCount = processingErrorCount;
            Children = [];
        }

        public CitilinkCatalogBranchEntity(string url,
            List<CitilinkCatalogBranchEntity> children,
            bool scrapeTargetsProcessed, CitilinkNodeType nodeType, int id = default,
            int processingErrorCount = 0)
            : this(url, scrapeTargetsProcessed, nodeType, id, processingErrorCount)
        {
            Children = children;
        }

        public string Url { get; set; }
        public List<CitilinkCatalogBranchEntity> Children
        { get; set; }

        public bool ScrapeTargetsProcessed { get; set; }
        public CitilinkNodeType NodeType { get; set; }
        public int ProcessingErrorCount { get; set; }

        public override string ToString()
        {
            return $"CitilinkBranchEntity: Id is {Id}, Url is {Url}, Children count is {Children.Count}, " +
                $"AreDescendantsProcessed: {ScrapeTargetsProcessed}" +
                $" NodeType is {NodeType}, ProcessingErrorCount is {ProcessingErrorCount}";
        }
    }
}
