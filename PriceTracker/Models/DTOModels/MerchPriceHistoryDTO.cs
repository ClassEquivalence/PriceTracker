using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.DTOModels
{
    public record MerchPriceHistoryDTO: BaseDTO
    {
        public List<TimestampedPriceDTO> TimestampedPricesList { get; init; }
        public TimestampedPriceDTO CurrentPrice { get; init; }
        public MerchPriceHistoryDTO(TimestampedPriceDTO currentPrice, int id = default) :
            base(id)
        {
            CurrentPrice = currentPrice;
            TimestampedPricesList = [currentPrice];
        }
        public MerchPriceHistoryDTO(List<TimestampedPriceDTO> timestampedPrices, 
            TimestampedPriceDTO currentPrice, int id=default): base(id)
        {
            if(!timestampedPrices.Contains(currentPrice)) 
                timestampedPrices.Add(currentPrice);
            CurrentPrice = currentPrice;
            TimestampedPricesList = timestampedPrices;
        }
    }
}
