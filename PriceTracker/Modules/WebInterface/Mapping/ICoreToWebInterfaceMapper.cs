using PriceTracker.Core.Models;
using PriceTracker.Modules.WebInterface.DTOModels;

namespace PriceTracker.Modules.WebInterface.Mapping
{
    public interface ICoreToWebInterfaceMapper<in CoreDto, out WebDto>
        where CoreDto : BaseDto where WebDto : BaseWebInterfaceDto
    {
        WebDto Map(CoreDto dto);
    }
}
