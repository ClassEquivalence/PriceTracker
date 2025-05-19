using PriceTracker.Models.DomainModels.ShopSpecificModels.Citilink;
using PriceTracker.Models.DTOModels.ForParsing;
using PriceTracker.Models.Services.MerchDataExtraction;

namespace PriceTracker.Models.Services.MerchService.Citilink
{
    public interface ICitilinkMerchService: IMerchService<CitilinkMerch>, 
        IMerchDataConsumer<CitilinkMerchParsingDto>
    {
        public CitilinkMerch? GetByCitilinkId(string citilinkId);
    }
}
