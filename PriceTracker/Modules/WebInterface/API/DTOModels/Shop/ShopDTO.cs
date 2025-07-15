using PriceTracker.Modules.WebInterface.API.DTOModels.Merch;

namespace PriceTracker.Modules.WebInterface.API.DTOModels.Shop
{
    public record ShopDTO : BaseDomainWebInterfaceDto
    {
        public string Name { get; init; }
        public virtual List<MerchOverviewDto> Merches { get; init; }

        public ShopDTO(string name, List<MerchOverviewDto>? merches = null, int id = default) :
            base(id)
        {
            Name = name;
            Merches = merches ?? [];
        }
    }
}
