using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Merch;

namespace PriceTracker.Modules.WebInterface.Mapping.Merch
{
    public class OverviewMerchDtoMapper : IOverviewMerchDtoMapper
    {
        public MerchOverviewDto Map(MerchDto dto)
        {
            return new MerchOverviewDto(dto.Name, dto.PriceTrack.CurrentPrice.Price, dto.Id);
        }
    }
}
