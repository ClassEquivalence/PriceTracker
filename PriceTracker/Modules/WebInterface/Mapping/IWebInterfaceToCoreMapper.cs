using PriceTracker.Core.Models;
using PriceTracker.Modules.WebInterface.DTOModels;

namespace PriceTracker.Modules.WebInterface.Mapping
{
    public interface IWebInterfaceToCoreMapper<out CoreDto, in WebDto>
        where CoreDto : BaseDto where WebDto : BaseWebInterfaceDto
    {
        CoreDto Map(WebDto dto);
    }
}
