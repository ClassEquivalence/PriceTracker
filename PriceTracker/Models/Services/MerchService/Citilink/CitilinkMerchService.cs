using PriceTracker.Models.DomainModels.ShopSpecificModels.Citilink;

namespace PriceTracker.Models.Services.MerchService.Citilink
{
    public class CitilinkMerchService : ICitilinkMerchService
    {
        public MerchService baseMerchService;
        public CitilinkMerchService(MerchService baseMerchService)
        {
            this.baseMerchService = baseMerchService;
        }

        public CitilinkMerch GetByCitilinkId(string citilinkId)
        {
            baseMerchService.
        }

    }
}
