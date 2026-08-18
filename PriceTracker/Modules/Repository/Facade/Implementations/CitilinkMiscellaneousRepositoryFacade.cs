using PriceTracker.Core.Models.Infrastructure;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink;

namespace PriceTracker.Modules.Repository.Facade.Implementations
{
    public class CitilinkMiscellaneousRepositoryFacade : ICitilinkMiscellaneousRepositoryFacade
    {
        private readonly CitilinkExtractorStorageStateRepository
            _citilinkExtractorStorageStateRepository;
        private readonly CitilinkExtractionStateRepository
            _citilinkParsingExecutionStateRepository;
        private readonly ILogger? _logger;

        public CitilinkMiscellaneousRepositoryFacade(ILogger? logger,
            CitilinkExtractorStorageStateRepository citilinkStorageStateRepository,
            CitilinkExtractionStateRepository citilinkExtractionStateRepository)
        {
            _citilinkExtractorStorageStateRepository = citilinkStorageStateRepository;
            _citilinkParsingExecutionStateRepository = citilinkExtractionStateRepository;
            _logger = logger;
        }

        public CitilinkExtractorStorageStateDto? GetExtractorStorageState()
        {
            return _citilinkExtractorStorageStateRepository.
                Get();
        }

        public CitilinkExtractionStateDto? Provide()
        {
            return _citilinkParsingExecutionStateRepository.Get();
        }

        public void Save(CitilinkExtractionStateDto info)
        {
            _logger?.LogTrace($"{nameof(CitilinkMiscellaneousRepositoryFacade)}: " +
                $"{info} is getting saved.");
            _citilinkParsingExecutionStateRepository.Set(info);
        }

        public void SetExtractorStorageState(CitilinkExtractorStorageStateDto storageStateDto)
        {
            _citilinkExtractorStorageStateRepository.
                Set(storageStateDto);
        }

        /// <summary>
        /// Updateable branch should have parent.
        /// </summary>
        /// <param name="branch"></param>
        public void UpdateBranch(CitilinkBranch branch)
        {
            var extractionState = Provide();
            var extractionStateTree = extractionState.CachedUrls;
            var parent = extractionStateTree.GetAllBranches().
                SingleOrDefault(b => b.Id == branch.ParentId);
            if (parent == null)
            {
                if (branch.Url != "https://www.citilink.ru/catalog/")
                    throw new InvalidOperationException("Root branches url doesn't match expected url");
                extractionState.CachedUrls.Root = branch;
                _logger?.LogTrace($"{nameof(UpdateBranch)} We're updating root branch, seemingly.");
            }
            else
            {
                var removable = parent.Children.Single(b => b.Id == branch.Id);

                parent.Children.Remove(removable);
                parent.Children.Add(branch with { Children = removable.Children });
            }
            Save(extractionState);
        }
    }
}
