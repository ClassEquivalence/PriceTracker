using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.WebInterface.DTOModels.ForAPI.Merch;

namespace PriceTracker.Modules.WebInterface.Mapping.Merch
{
    public class DetailedMerchDtoMapper : IDetailedMerchDtoMapper
    {
        public MerchDto Map(DetailedMerchDto dto)
        {
            return new MerchDto(dto.Id, dto.Name, dto.MerchPriceHistory,
                dto.ShopId, dto.MerchPriceHistory.Id);
        }

        public DetailedMerchDto Map(MerchDto dto)
        {
            return new DetailedMerchDto(dto.PriceTrack, dto.Name, dto.ShopId,
                dto.Id);
        }
    }
}
