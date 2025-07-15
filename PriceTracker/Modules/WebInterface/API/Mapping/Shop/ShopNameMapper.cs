using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.API.DTOModels.Shop;

namespace PriceTracker.Modules.WebInterface.API.Mapping.Shop
{
    public class ShopNameMapper : IShopNameMapper
    {
        public ShopNameDto Map(ShopDto dto)
        {
            return new ShopNameDto(dto.Name, dto.Id);
        }
    }
}
