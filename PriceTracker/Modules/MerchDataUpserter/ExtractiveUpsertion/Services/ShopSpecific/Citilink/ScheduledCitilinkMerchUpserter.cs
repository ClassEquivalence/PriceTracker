using PriceTracker.Modules.MerchDataUpserter.Core.Models.ForParsing;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services;
using PriceTracker.Modules.Repository.Facade.Citilink;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink
{
    public class ScheduledCitilinkMerchUpserter :
        ScheduledMerchUpserter<CitilinkMerchParsingDto, CitilinkParsingExecutionState>
    {
        private readonly ICitilinkMiscellaneousRepositoryFacade _miscRepository;
        public ScheduledCitilinkMerchUpserter(IMerchDataConsumer<CitilinkMerchParsingDto>
            dataConsumer, GUICitilinkExtractor dataExtractor, TimeSpan upsertionCyclePeriod,
            DateTime upsertionStartTime, CitilinkParsingExecutionState executionState,
            ICitilinkMiscellaneousRepositoryFacade repository) :
            base(dataConsumer, dataExtractor, upsertionCyclePeriod, upsertionStartTime,
                executionState)
        {
            _miscRepository = repository;
        }

        public override async Task OnShutDown()
        {
            var baseShutDownTask = base.OnShutDown();
            var getStorageStateTask = ((GUICitilinkExtractor)_dataExtractor).
                GetScraperStorageStateAsync();

            string storageState = await getStorageStateTask;
            _miscRepository.SetExtractorStorageState(new(storageState));
            await baseShutDownTask;
        }
    }
}
