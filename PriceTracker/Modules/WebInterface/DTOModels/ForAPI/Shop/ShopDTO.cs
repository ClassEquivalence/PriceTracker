using Microsoft.OpenApi.Writers;
using PriceTracker.Models.DomainModels;
using PriceTracker.Modules.WebInterface.DTOModels;

namespace PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Shop
{
    public record ShopDTO : BaseWebInterfaceDto
    {
        public string Name { get; init; }
        public virtual List<MerchDTO> Merches { get; init; }

        public ShopDTO(string name, List<MerchDTO>? merches = null, int id = default) :
            base(id)
        {
            Name = name;
            Merches = merches ?? [];
        }
    }
}
