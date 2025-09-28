using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Modules.Repository.Entities.Process.ShopSpecific.Extraction.CatalogTree;

namespace PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink.ExtractionState
{
    public interface ICitilinkCatalogUrlTreeMapper: ICoreToEntityMapper
        <CatalogUrlsTree, CitilinkCatalogUrlsTreeEntity>
    {
    }
}
