using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction.CatalogTree;

namespace PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink.ExtractionState
{
    public class CitilinkCatalogUrlTreeMapper : ICoreToEntityMapper
        <CatalogUrlsTree, CitilinkCatalogUrlsTreeEntity>
    {
        private readonly ICoreToEntityMapper<Branch,
            CitilinkCatalogBranchEntity> _branchMapper;
        public CitilinkCatalogUrlTreeMapper(ICoreToEntityMapper<Branch,
            CitilinkCatalogBranchEntity> branchMapper)
        {
            _branchMapper = branchMapper;
        }
        public CatalogUrlsTree Map(CitilinkCatalogUrlsTreeEntity entity)
        {
            return new(_branchMapper.Map(entity.Root), entity.Id);
        }

        public CitilinkCatalogUrlsTreeEntity Map(CatalogUrlsTree model)
        {
            return new(_branchMapper.Map(model.Root), model.Id);
        }
    }
}
