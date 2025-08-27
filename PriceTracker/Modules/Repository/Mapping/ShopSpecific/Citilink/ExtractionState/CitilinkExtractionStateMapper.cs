using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction.CatalogTree;

namespace PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink.ExtractionState
{
    /// <summary>
    /// Эта реализация маппера не следит за единичностью инстансов модели
    /// и просто создаёт новые при каждом вызове маппинга.
    /// </summary>
    public class CitilinkExtractionStateMapper : ICoreToEntityMapper
        <CitilinkExtractionStateDto, CitilinkParsingExecutionStateEntity>
    {
        private readonly ICoreToEntityMapper
            <CatalogUrlsTree, CitilinkCatalogUrlsTreeEntity> _urlTreeMapper;
        public CitilinkExtractionStateMapper(ICoreToEntityMapper
            <CatalogUrlsTree, CitilinkCatalogUrlsTreeEntity> urlTreeMapper)
        {
            _urlTreeMapper = urlTreeMapper;
        }

        public CitilinkExtractionStateDto Map(CitilinkParsingExecutionStateEntity entity)
        {
            CatalogUrlsTree tree = _urlTreeMapper.Map(entity.CatalogUrls);
            CitilinkExtractionStateDto dto = new(tree, entity.IsCompleted);
            return dto;
        }

        public CitilinkParsingExecutionStateEntity Map(CitilinkExtractionStateDto model)
        {
            CitilinkCatalogUrlsTreeEntity tree = _urlTreeMapper.Map(model.CachedUrls);
            CitilinkParsingExecutionStateEntity entity = new(model.IsCompleted);
            entity.CatalogUrls = tree;
            return entity;
        }
    }
}
