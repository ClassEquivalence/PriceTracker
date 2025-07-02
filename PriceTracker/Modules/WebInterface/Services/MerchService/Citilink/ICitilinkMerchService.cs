using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;

namespace PriceTracker.Modules.WebInterface.Services.MerchService.Citilink
{
    public interface ICitilinkMerchService : IMerchService<CitilinkMerchDto>
    {
        public CitilinkMerchDto? GetByCitilinkId(string citilinkId);
    }
}
