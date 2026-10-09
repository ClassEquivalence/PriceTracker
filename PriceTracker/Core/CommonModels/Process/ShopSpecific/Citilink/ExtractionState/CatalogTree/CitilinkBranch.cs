using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models;

namespace PriceTracker.Core.CommonModels.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree
{
    public record CitilinkBranch : BaseDto
    {
        public int Id;
        // Url сопоставляется дефолтному ScrapeTarget
        public string Url;
        public CitilinkBranch? Parent;
        public int? ParentId;
        public List<CitilinkBranch> Children;
        public bool DescendantsProcessed;
        public CitilinkNodeAugmentationState AugmentationState;
        public CitilinkNodeType NodeType;
        public int ProcessingErrorCount;
        /*
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
        */

        public CitilinkBranch(int id, string url, List<CitilinkBranch> children,
             bool descendantsProcessed, CitilinkNodeType nodeType,
             CitilinkNodeAugmentationState augmentationState, CitilinkBranch? parent = null,
            int? parentId = null, int processingErrorCount = 0)
        {
            Id = id;
            Url = url;
            Children = children;
            DescendantsProcessed = descendantsProcessed;
            NodeType = nodeType;
            AugmentationState = augmentationState;
            Parent = parent;
            ParentId = parentId;
            ProcessingErrorCount = processingErrorCount;
        }

        public CitilinkBranch DeepClone()
        {
            return new CitilinkBranch(Id, Url, Children.Select(c => c.DeepClone()).ToList(),
                DescendantsProcessed, NodeType, AugmentationState, Parent, ParentId);
        }
    }
}
