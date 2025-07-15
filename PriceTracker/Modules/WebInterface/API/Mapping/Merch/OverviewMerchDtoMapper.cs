using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.API.DTOModels.Merch;

namespace PriceTracker.Modules.WebInterface.API.Mapping.Merch
{
    public class OverviewMerchDtoMapper : IOverviewMerchDtoMapper
    {
        public MerchOverviewDto Map(MerchDto dto)
        {
            return new MerchOverviewDto(dto.Name, dto.PriceTrack.CurrentPrice.Price, dto.Id);
        }
    }
}
