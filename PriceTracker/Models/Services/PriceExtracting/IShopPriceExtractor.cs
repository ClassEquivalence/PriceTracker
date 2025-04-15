using PriceTracker.Models.DomainModels;

namespace PriceTracker.Models.Services.PriceExtracting
{
    public interface IShopPriceExtractor
    {
        public TimestampedPrice Extract(IMerchModel merch);
    }
}
