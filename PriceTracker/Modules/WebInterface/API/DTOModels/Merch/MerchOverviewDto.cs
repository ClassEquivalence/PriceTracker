namespace PriceTracker.Modules.WebInterface.API.DTOModels.Merch
{
    public record MerchOverviewDto : BaseDomainWebInterfaceDto
    {
        public string Name { get; init; }
        public decimal CurrentPrice { get; init; }

        public MerchOverviewDto(string name, decimal currentPrice, int id = default) :
            base(id)
        {
            Name = name;
            CurrentPrice = currentPrice;
        }
    }
}
