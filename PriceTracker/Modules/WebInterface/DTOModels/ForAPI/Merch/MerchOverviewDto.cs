using PriceTracker.Modules.WebInterface.DTOModels;

namespace PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Merch
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
