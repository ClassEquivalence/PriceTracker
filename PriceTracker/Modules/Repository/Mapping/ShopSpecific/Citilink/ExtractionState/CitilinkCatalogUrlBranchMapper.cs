using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction.CatalogTree;

namespace PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink.ExtractionState
{
    public class CitilinkCatalogUrlBranchMapper : ICitilinkCatalogUrlBranchMapper
    {
        private ILogger? _logger;

        public CitilinkCatalogUrlBranchMapper(ILogger? logger)
        {
            _logger = logger;
        }

        public CitilinkBranch Map(CitilinkCatalogBranchEntity entity)
        {
            return new CitilinkBranch(entity.Id, entity.Url, entity.Children != null ?
                entity.Children.Select(Map).ToList() : [], entity.DescendantsProcessed,
                entity.NodeType, entity.AugmentationState, processingErrorCount: 
                entity.ProcessingErrorCount);
        }

        public CitilinkCatalogBranchEntity Map(CitilinkBranch model)
        {
            var entity = new CitilinkCatalogBranchEntity(model.Url, model.Children.Select(Map).ToList(),
                model.DescendantsProcessed, model.AugmentationState, model.NodeType, model.Id,
                processingErrorCount: model.ProcessingErrorCount);
            _logger?.LogTrace($"Branch model to entity: Model - {model}\n" +
                $"Entity - {entity}");
            return entity;
        }
    }
}
