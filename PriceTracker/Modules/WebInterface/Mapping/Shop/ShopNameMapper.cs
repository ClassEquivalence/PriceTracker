using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Shop;

namespace PriceTracker.Modules.WebInterface.Mapping.Shop
{
    public class ShopNameMapper : IShopNameMapper
    {
        public ShopNameDto Map(ShopDto dto)
        {
            return new ShopNameDto(dto.Name, dto.Id);
        }
    }
}
