namespace PriceTracker.Core.Models.Domain.ShopSpecific.Citilink
{
    public record CitilinkMerchDto(int Id, string Name, MerchPriceHistoryDto PriceTrack,
        string CitilinkId, int PriceHistoryId) : MerchDto(Id, Name, PriceTrack,
            PriceHistoryId);
}
