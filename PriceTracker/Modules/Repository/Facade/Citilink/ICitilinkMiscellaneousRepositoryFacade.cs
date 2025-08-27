using PriceTracker.Core.Models.Infrastructure;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;

namespace PriceTracker.Modules.Repository.Facade.Citilink
{
    public interface ICitilinkMiscellaneousRepositoryFacade :
        IExtractionExecutionStateProvider<CitilinkExtractionStateDto>
    {
        public void SetExtractorStorageState(CitilinkExtractorStorageStateDto storageStateDto);
        public CitilinkExtractorStorageStateDto? GetExtractorStorageState();
    }
}
