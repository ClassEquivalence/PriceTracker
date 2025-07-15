namespace PriceTracker.Core.Models.Process.ShopSpecific.Citilink
{
    public record CitilinkExtractionStateDto(bool IsCompleted,
        string CurrentCatalogUrl, int CatalogPageNumber) :
        ExtractionStateDto(IsCompleted);
}
