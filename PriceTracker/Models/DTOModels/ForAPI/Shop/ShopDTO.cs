using Microsoft.OpenApi.Writers;
using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DTOModels.ForAPI.Shop
{
    public record ShopDTO : BaseDTO
    {
        public string Name { get; init; }
        public virtual List<MerchDTO> Merches { get; init; }

        public ShopDTO(string name, List<MerchDTO>? merches = null, int id = default):
            base(id)
        {
            Name = name;
            Merches = merches ?? [];
        }
    }
}
