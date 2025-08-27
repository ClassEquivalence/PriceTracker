using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction.CatalogTree;

namespace PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink.ExtractionState
{
    public class CitilinkCatalogUrlBranchMapper : ICoreToEntityMapper
        <Branch, CitilinkCatalogBranchEntity>
    {
        private ILogger? _logger;

        public CitilinkCatalogUrlBranchMapper(ILogger? logger)
        {
            _logger = logger;
        }

        public Branch Map(CitilinkCatalogBranchEntity entity)
        {
            return new Branch(entity.Id, entity.CurrentCatalogUrl, entity.Branches != null? 
                entity.Branches.Select(Map).ToList(): [], entity.IsBranchProcessed);
        }

        public CitilinkCatalogBranchEntity Map(Branch model)
        {
            _logger?.LogTrace($"{nameof(CitilinkCatalogUrlBranchMapper)}: url ветви: {model.Url}");
            return new CitilinkCatalogBranchEntity(model.Url,
                model.Children .Select(Map).ToList(), model.IsProcessed, model.Id);
        }
    }
}
