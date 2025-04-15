using PriceTracker.Models.DomainModels;
using PriceTracker.Models.DTOModels;
using PriceTracker.Models.DTOModels.ForAPI.Merch;

namespace PriceTracker.Models.Services.Mapping.MicroMappers
{
    public class MerchToDtoMapper : IMerchToDtoMapper
    {
        public MerchOverviewDto ToMerchOverview(MerchModel merch)
        {
            return new(merch.Name, merch.CurrentPrice.Price, merch.Id);
        }
        public DetailedMerchDto ToDetailedMerch(MerchModel merch)
        {
            return new(ToPriceHistoryDto(merch.PriceTrack), merch.Name, merch.ShopId, merch.Id);
        }

        protected MerchPriceHistoryDTO ToPriceHistoryDto(MerchPriceHistory model)
        {
            var timestampedPrices = model.TimestampedPrices.Select(ToTimestampedPriceDto).ToList();
            return new(timestampedPrices, ToTimestampedPriceDto(model.CurrentPrice), model.Id);
        }
        protected TimestampedPriceDTO ToTimestampedPriceDto(TimestampedPrice model)
        {
            return new(model.Price, model.DateTime, model.Id);
        }
    }
}
