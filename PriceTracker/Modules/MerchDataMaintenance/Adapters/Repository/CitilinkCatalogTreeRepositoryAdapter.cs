using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.Repository.Facade.Citilink;

namespace PriceTracker.Modules.MerchDataMaintenance.Adapters.Repository
{
    public class CitilinkCatalogTreeRepositoryAdapter : ICitilinkCatalogTreeRepositoryAdapter
    {
        ICitilinkMiscellaneousRepositoryFacade _repositoryFacade;
        private const string mainCatalogUrl = "https://www.citilink.ru/catalog/";
        private readonly ILogger? _logger;
        private List<CitilinkNode> _retreivedNodeCache;

        public CitilinkCatalogTreeRepositoryAdapter(
            ICitilinkMiscellaneousRepositoryFacade repository, ILogger? logger)
        {
            _repositoryFacade = repository;
            _logger = logger;
            _retreivedNodeCache = [];
        }

        public async Task AppendBranchesAsync(CitilinkNode parent,
            List<CitilinkScrapeTarget> descendants)
        {
            CheckForRepetitions();
            var state = _repositoryFacade.Provide();
            var allBranches = state.CachedUrls.GetAllBranches();
            var parentBranch = allBranches
                .Single(b => b.Url == parent.DefaultLocation.Url);

            var uniqueUrlDescendants = descendants.Where(st =>
            !allBranches.Any(b => b.Url == st.Url));
            uniqueUrlDescendants = uniqueUrlDescendants.DistinctBy(b => b.Url);

            var targetsAsBranches = uniqueUrlDescendants.Select(st => new CitilinkBranch(default, st.Url,
                [], false, CitilinkNodeType.Unknown, CitilinkNodeAugmentationState.Unprocessed,
                parentBranch, parentBranch.Id));
            parentBranch.Children.AddRange(targetsAsBranches);

            _repositoryFacade.Save(state);
            CheckForRepetitions();
        }

        public async Task<CitilinkNode?> GetUnprocessedNodeAsync(int maxErrorsCount = -1)
        {
            if (_retreivedNodeCache.All(nc => nc.AugmentationState
            == CitilinkNodeAugmentationState.Processed))
            {
                var branches = _repositoryFacade.Provide().CachedUrls.GetAllBranches().
                Where(b => b.AugmentationState != CitilinkNodeAugmentationState.Processed
                // TODO: убрать хардкод-константу -1. -1 - означает что максимум не задан.
                && (b.ProcessingErrorCount < maxErrorsCount || maxErrorsCount == -1));

                _retreivedNodeCache = branches.Select(Map).ToList();
            }

            return _retreivedNodeCache.Find(cn => cn.AugmentationState !=
            CitilinkNodeAugmentationState.Processed);
        }

        public async Task<CitilinkNode> GetRootAsync()
        {
            var extractionState = _repositoryFacade.Provide();
            var tree = extractionState.CachedUrls;
            return Map(tree.Root);
        }

        public async Task RecreateTreeAsync()
        {
            CitilinkBranch root = new(default, mainCatalogUrl, [], false, CitilinkNodeType.RootCatalog,
                CitilinkNodeAugmentationState.PartiallyProcessed_TypeIsKnown);
            CatalogUrlsTree tree = new(root);

            _logger?.LogTrace($"Tree's being recreated/created, tree is: {tree}");

            CitilinkExtractionStateDto extractionState = new(tree, false);
            _repositoryFacade.Save(extractionState);
        }


        private static CitilinkNode Map(CitilinkBranch branch)
        {
            CitilinkScrapeTarget target = new(branch.Url);
            CitilinkNode node = new(target, branch.AugmentationState, branch.NodeType,
                branch.Children.Select(Map).ToList(), branch.DescendantsProcessed,
                processingErrorCount: branch.ProcessingErrorCount);
            return node;
        }

        private CitilinkBranch Map(CitilinkNode node, int id,
            int? ParentId)
        {
            var matchingBranch = GetMatchingBranch(node);
            CitilinkBranch branch = new(id, node.DefaultLocation.Url,
                node.Descendants.Select(n => Map(n, GetMatchingBranch(n).Id, id)).
                ToList(), node.DescendantsProcessed,
                node.NodeType, node.AugmentationState, processingErrorCount:
                node.ProcessingErrorCount);
            branch.ParentId = ParentId;

            return branch;
        }

        private CitilinkBranch GetMatchingBranch(CitilinkNode node)
        {
            var extractionState = _repositoryFacade.Provide();
            var allBranches = extractionState.CachedUrls.GetAllBranches();
            var matchingBranch = allBranches.
                Where(b => b.Url == node.DefaultLocation.Url).Single();
            bool thereIsParentBranch = allBranches.Any
                (b => b.Children.Any(child => child.Id == matchingBranch.Id));
            if (thereIsParentBranch)
            {
                var parentBranch = allBranches.Where
                    (b => b.Children.Any(child => child.Id == matchingBranch.Id))
                    .Single();
                matchingBranch.ParentId = parentBranch.Id;
            }
            return matchingBranch;
        }

        public async Task UpdateNodeAsync(CitilinkNode node)
        {
            CheckForRepetitions();
            var matchingBranch = GetMatchingBranch(node);
            var updatedBranch = Map(node, matchingBranch.Id, matchingBranch.ParentId);
            _repositoryFacade.UpdateBranch(updatedBranch);
            CheckForRepetitions();
        }

        private void CheckForRepetitions()
        {
            var extractionState = _repositoryFacade.Provide();
            var allBranches = extractionState.CachedUrls.GetAllBranches();
            if(allBranches.DistinctBy(b=>b.Url).Count() < allBranches.Count)
            {
                throw new InvalidOperationException("There are url repetitions");
            }
        }
    }
}
