using PriceTracker.Core.Models.Infrastructure;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Repositories.ShopSpecific.Citilink;

namespace PriceTracker.Modules.Repository.Facade.Implementations
{
    public class CitilinkMiscellaneousRepositoryFacade : ICitilinkMiscellaneousRepositoryFacade
    {
        private readonly CitilinkExtractorStorageStateRepository
            _citilinkExtractorStorageStateRepository;
        private readonly CitilinkExtractionStateRepository
            _citilinkParsingExecutionStateRepository;

        public CitilinkMiscellaneousRepositoryFacade(
            CitilinkExtractorStorageStateRepository citilinkStorageStateRepository,
            CitilinkExtractionStateRepository citilinkExtractionStateRepository)
        {
            _citilinkExtractorStorageStateRepository = citilinkStorageStateRepository;
            _citilinkParsingExecutionStateRepository = citilinkExtractionStateRepository;
        }

        public CitilinkExtractorStorageStateDto? GetExtractorStorageState()
        {
            return _citilinkExtractorStorageStateRepository.
                Get();
        }

        public CitilinkExtractionStateDto? Provide()
        {
            return _citilinkParsingExecutionStateRepository.Get();
        }

        public void Save(CitilinkExtractionStateDto info)
        {
            _citilinkParsingExecutionStateRepository.Set(info);
        }

        public void SetExtractorStorageState(CitilinkExtractorStorageStateDto storageStateDto)
        {
            _citilinkExtractorStorageStateRepository.
                Set(storageStateDto);
        }
    }
}
