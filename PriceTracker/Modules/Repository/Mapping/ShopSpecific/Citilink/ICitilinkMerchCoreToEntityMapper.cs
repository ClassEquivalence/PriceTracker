using PriceTracker.Core.Models.Domain.ShopSpecific.Citilink;
using PriceTracker.Modules.Repository.Entities.Domain.ShopSpecific;
using PriceTracker.Modules.Repository.Mapping.Domain;

namespace PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink
{
    public interface ICitilinkMerchCoreToEntityMapper :
        IMerchCoreToEntityMapper<CitilinkMerchDto, CitilinkMerchEntity>
    {
    }
}
