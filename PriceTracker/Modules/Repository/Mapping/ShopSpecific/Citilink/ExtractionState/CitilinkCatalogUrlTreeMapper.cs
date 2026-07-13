using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction.CatalogTree;

namespace PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink.ExtractionState
{
    public class CitilinkCatalogUrlTreeMapper : ICitilinkCatalogUrlTreeMapper
    {
        private readonly ICoreToEntityMapper<CitilinkBranch,
            CitilinkCatalogBranchEntity> _branchMapper;

        private readonly ILogger? _logger;
        public CitilinkCatalogUrlTreeMapper(ICitilinkCatalogUrlBranchMapper branchMapper,
            ILogger? logger)
        {
            _branchMapper = branchMapper;
            _logger = logger;
        }
        public CatalogUrlsTree Map(CitilinkCatalogUrlsTreeEntity entity)
        {
            CatalogUrlsTree tree = new(_branchMapper.Map(entity.Root), entity.Id);
            _logger?.LogTrace($"CatalogUrlsTree's being mapped:" +
                $" entity -> model, {tree}");
            return tree;
        }

        public CitilinkCatalogUrlsTreeEntity Map(CatalogUrlsTree model)
        {
            CitilinkCatalogUrlsTreeEntity treeEntity = new(_branchMapper.Map(model.Root), model.Id);
            _logger?.LogTrace($"CatalogUrlsTree's being mapped:" +
                $" model -> entity, {treeEntity}");
            return treeEntity;
        }
    }
}
