using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;

namespace PriceTracker.Modules.Repository.Facade.Citilink
{
    public interface ICitilinkMerchRepositoryFacade
    {
        List<CitilinkMerchDto> Where(Func<CitilinkMerchDto, bool> selector);

        bool TryGetSingleByCitilinkId(string citilinkId, out CitilinkMerchDto? dto);
        bool TryUpdate(CitilinkMerchDto dto);
        bool TryInsert(CitilinkMerchDto dto);
    }
}
