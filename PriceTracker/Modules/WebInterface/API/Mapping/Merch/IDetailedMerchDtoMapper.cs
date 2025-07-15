using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.API.DTOModels.Merch;

namespace PriceTracker.Modules.WebInterface.API.Mapping.Merch
{
    public interface IDetailedMerchDtoMapper : IBidirectionalCoreToWebInterfaceDtoMapper<MerchDto, DetailedMerchDto>
    {
    }
}
