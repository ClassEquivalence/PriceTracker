using PriceTracker.Core.Models;
using PriceTracker.Modules.WebInterface.DTOModels;

namespace PriceTracker.Modules.WebInterface.Mapping
{
    public interface IBidirectionalCoreToWebInterfaceDtoMapper<CoreDto, WebDto> :
        ICoreToWebInterfaceMapper<CoreDto, WebDto>,
        IWebInterfaceToCoreMapper<CoreDto, WebDto>
        where CoreDto : BaseDto where WebDto : BaseWebInterfaceDto
    {

    }
}
