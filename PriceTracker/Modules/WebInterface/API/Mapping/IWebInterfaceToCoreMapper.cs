using PriceTracker.Core.Models;
using PriceTracker.Modules.WebInterface.API.DTOModels;

namespace PriceTracker.Modules.WebInterface.API.Mapping
{
    public interface IWebInterfaceToCoreMapper<out CoreDto, in WebDto>
        where CoreDto : BaseDto where WebDto : BaseWebInterfaceDto
    {
        CoreDto Map(WebDto dto);
    }
}
