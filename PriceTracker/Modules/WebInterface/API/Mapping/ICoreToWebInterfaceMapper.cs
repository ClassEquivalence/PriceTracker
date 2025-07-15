using PriceTracker.Core.Models;
using PriceTracker.Modules.WebInterface.API.DTOModels;

namespace PriceTracker.Modules.WebInterface.API.Mapping
{
    public interface ICoreToWebInterfaceMapper<in CoreDto, out WebDto>
        where CoreDto : BaseDto where WebDto : BaseWebInterfaceDto
    {
        WebDto Map(CoreDto dto);
    }
}
