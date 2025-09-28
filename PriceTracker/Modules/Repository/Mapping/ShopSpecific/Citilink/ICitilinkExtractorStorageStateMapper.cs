using PriceTracker.Core.Models.Infrastructure;
using PriceTracker.Modules.Repository.Entities.Infrastructure;

namespace PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink
{
    public interface ICitilinkExtractorStorageStateMapper :
        ICoreToEntityMapper<CitilinkExtractorStorageStateDto,
                CitilinkExtractorStorageStateEntity>
    {
    }
}
