using PriceTracker.Core.Models.Infrastructure;
using PriceTracker.Modules.Repository.Entities.Infrastructure;

namespace PriceTracker.Modules.Repository.Mapping.ShopSpecific.Citilink
{
    public class CitilinkExtractorStorageStateMapper :
        ICoreToEntityMapper<CitilinkExtractorStorageStateDto,
                CitilinkExtractorStorageStateEntity>
    {
        public CitilinkExtractorStorageStateDto Map(CitilinkExtractorStorageStateEntity entity)
        {
            return new(entity.StorageState);
        }

        public CitilinkExtractorStorageStateEntity Map(CitilinkExtractorStorageStateDto model)
        {
            return new(default, model.StorageState);
        }
    }
}
