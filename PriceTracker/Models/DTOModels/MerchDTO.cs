using PriceTracker.Models.DTOModels.ForAPI.Shop;

namespace PriceTracker.Models.DTOModels
{
    public record MerchDTO: BaseDTO
    {
        public string Name { get; init; }
        public MerchPriceHistoryDTO PriceHistory { get; init; }

        public ShopDTO Shop { get; init; }

        public MerchDTO(string name, ShopDTO shop, MerchPriceHistoryDTO priceHistory,
            int id = default): base(id)
        {
            Name = name;
            Shop = shop;
            PriceHistory = priceHistory;
        }
    }
}
