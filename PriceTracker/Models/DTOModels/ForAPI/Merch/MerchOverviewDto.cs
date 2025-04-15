namespace PriceTracker.Models.DTOModels.ForAPI.Merch
{
    public record MerchOverviewDto: BaseDTO
    {
        public string Name { get; init; }
        public decimal CurrentPrice { get; init; }

        public MerchOverviewDto(string name, decimal currentPrice, int id = default):
            base(id)
        {
            Name = name;
            CurrentPrice = currentPrice;
        }
    }
}
