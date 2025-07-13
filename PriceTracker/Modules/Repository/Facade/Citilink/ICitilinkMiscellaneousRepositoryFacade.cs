using PriceTracker.Core.Models.Infrastructure;

namespace PriceTracker.Modules.Repository.Facade.Citilink
{
    public interface ICitilinkMiscellaneousRepositoryFacade
    {
        public void SetExtractorStorageState(CitilinkExtractorStorageStateDto storageStateDto);
        public CitilinkExtractorStorageStateDto GetExtractorStorageState();
    }
}
