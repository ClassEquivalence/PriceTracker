using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Merch;

namespace PriceTracker.Modules.WebInterface.Mapping.Merch
{
    public interface IDetailedMerchDtoMapper: IBidirectionalCoreToWebInterfaceDtoMapper<MerchDto, DetailedMerchDto>
    {
    }
}
