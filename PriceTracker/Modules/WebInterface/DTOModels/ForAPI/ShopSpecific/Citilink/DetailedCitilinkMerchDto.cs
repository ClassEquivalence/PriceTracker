using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Merch;

namespace PriceTracker.Modules.WebInterface.DTOModels.ForAPI.ShopSpecific.Citilink
{
    public record DetailedCitilinkMerchDto(string citilinkMerchId, MerchPriceHistoryDto priceHistory,
        string name, int shopId, int merchId = default) :
        DetailedMerchDto(priceHistory, name, shopId, merchId);
}
