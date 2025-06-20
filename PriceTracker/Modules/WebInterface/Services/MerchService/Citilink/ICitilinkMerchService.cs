using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Models.DomainModels.ShopSpecificModels.Citilink;
using PriceTracker.Modules.MerchDataProvider.MerchDataExtraction;
using PriceTracker.Modules.MerchDataProvider.Models.ForParsing;
using PriceTracker.Modules.WebInterface.Services.MerchService;

namespace PriceTracker.Modules.WebInterface.Services.MerchService.Citilink
{
    public interface ICitilinkMerchService : IMerchService<CitilinkMerchDto>,
        IMerchDataConsumer<CitilinkMerchParsingDto>
    {
        public CitilinkMerchDto? GetByCitilinkId(string citilinkId);
    }
}
