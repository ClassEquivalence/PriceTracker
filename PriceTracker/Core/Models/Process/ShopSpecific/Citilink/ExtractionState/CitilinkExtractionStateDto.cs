using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;

namespace PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState
{

    public record CitilinkExtractionStateDto: ExtractionStateDto
    {

        public CatalogUrlsTree? CachedUrls { get; set; }

        public CitilinkExtractionStateDto(CatalogUrlsTree? cachedUrls, bool isCompleted)
            :base(isCompleted)
        {
            CachedUrls = cachedUrls;
        }

        public CitilinkExtractionStateDto DeepClone()
        {
            return new(CachedUrls?.DeepClone() ?? null, IsCompleted);
        }
    }
}
