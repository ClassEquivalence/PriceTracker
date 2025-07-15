using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.API.DTOModels.Merch;

namespace PriceTracker.Modules.WebInterface.API.DTOModels.ShopSpecific.Citilink
{
    public record DetailedCitilinkMerchDto(string citilinkMerchId, MerchPriceHistoryDto priceHistory,
        string name, int shopId, int merchId = default) :
        DetailedMerchDto(priceHistory, name, shopId, merchId);
}
