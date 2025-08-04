using PriceTracker.Core.Models.Process.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.Core.Models.ForParsing;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink
{
    public class ScheduledCitilinkMerchUpserter :
        ScheduledMerchUpserter<CitilinkMerchParsingDto, CitilinkExtractionStateDto>
    {
        private readonly ICitilinkMiscellaneousRepositoryFacade _miscRepository;
        private readonly ExtractionStateSavePolicy _extractionStateSavePolicy;
        private readonly StorageStateSavePolicy _storageStateSavePolicy;

        private CitilinkExtractionStateDto _parsingExecutionState;


        [Flags]
        public enum ExtractionStateSavePolicy
        {
            None = 0,
            OnChange = 1 << 0,
            OnServerShutdown = 1 << 1,
        }

        [Flags]
        public enum StorageStateSavePolicy
        {
            None = 0,
            OnServerShutdown = 1 << 1,
        }


        public ScheduledCitilinkMerchUpserter(CitilinkMerchDataUpserter
            dataConsumer, GUICitilinkExtractor dataExtractor, TimeSpan upsertionCyclePeriod,
            DateTime upsertionStartTime, CitilinkExtractionStateDto executionState,
            ICitilinkMiscellaneousRepositoryFacade repository, ILogger? logger, ExtractionStateSavePolicy
            extractionStateSavePolicy = ExtractionStateSavePolicy.OnServerShutdown,
            StorageStateSavePolicy storageStateSavePolicy = StorageStateSavePolicy.OnServerShutdown) :
            base(dataConsumer, dataExtractor, upsertionCyclePeriod, upsertionStartTime,
                executionState, logger)
        {
            _miscRepository = repository;
            _extractionStateSavePolicy = extractionStateSavePolicy;
            _storageStateSavePolicy = storageStateSavePolicy;

            _parsingExecutionState = _miscRepository.Provide() with { };

            if ((extractionStateSavePolicy & ExtractionStateSavePolicy.OnChange) != 0)
            {
                dataConsumer.MerchPortionUpserted += () =>
                {
                    UpdateExtractionProgress();
                    DataConsumer_OnMerchPortionUpserted();
                };
            }
            else
            {
                dataConsumer.MerchPortionUpserted += UpdateExtractionProgress;
            }


        }

        private void UpdateExtractionProgress()
        {
            _parsingExecutionState = _miscRepository.Provide() with { };
        }

        private void DataConsumer_OnMerchPortionUpserted()
        {
            SaveExtractionProgress();
        }

        private void DataExtractor_OnExecutionStateUpdate(CitilinkExtractionStateDto obj)
        {
            SaveExtractionProgress(obj);
        }

        public override async Task OnShutDown()
        {
            //var baseShutDownTask = base.OnShutDown();

            if ((_extractionStateSavePolicy & ExtractionStateSavePolicy.OnServerShutdown) != 0)
            {
                SaveExtractionProgress();
            }
            if ((_storageStateSavePolicy & StorageStateSavePolicy.OnServerShutdown) != 0)
            {
                // TODO: Можно чуть больше реализовать асинхронность - но мне пока лень.
                var saveStorageTask = SaveStorageState();
                await saveStorageTask;
            }

            //await baseShutDownTask;
        }

        public async Task SaveStorageState()
        {
            var getStorageStateTask = ((GUICitilinkExtractor)_dataExtractor).
                GetScraperStorageStateAsync();

            string storageState = await getStorageStateTask;
            _miscRepository.SetExtractorStorageState(new(storageState));
        }

        public void SaveExtractionProgress()
        {
            SaveExtractionProgress(_parsingExecutionState);
        }
        private void SaveExtractionProgress(CitilinkExtractionStateDto progress)
        {
            var progressDto = new CitilinkExtractionStateDto(progress.IsCompleted,
                progress.CurrentCatalogUrl, progress.CatalogPageNumber);

            ((IExtractionExecutionStateProvider<CitilinkExtractionStateDto>)_miscRepository).
                Save(progressDto);

        }
    }
}
