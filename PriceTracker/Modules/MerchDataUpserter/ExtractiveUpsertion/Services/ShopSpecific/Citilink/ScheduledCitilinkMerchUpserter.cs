using PriceTracker.Core.Models.Process.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.Core.Models.ForParsing;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services;
using PriceTracker.Modules.Repository.Facade;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Repositories.Base;
using System;
using static PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink.ScheduledCitilinkMerchUpserter;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink
{
    public class ScheduledCitilinkMerchUpserter :
        ScheduledMerchUpserter<CitilinkMerchParsingDto, CitilinkParsingExecutionState>
    {
        private readonly ICitilinkMiscellaneousRepositoryFacade _miscRepository;
        private readonly ExtractionStateSavePolicy _extractionStateSavePolicy;
        private readonly StorageStateSavePolicy _storageStateSavePolicy;

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


        public ScheduledCitilinkMerchUpserter(IMerchDataConsumer<CitilinkMerchParsingDto>
            dataConsumer, GUICitilinkExtractor dataExtractor, TimeSpan upsertionCyclePeriod,
            DateTime upsertionStartTime, CitilinkParsingExecutionState executionState,
            ICitilinkMiscellaneousRepositoryFacade repository, ExtractionStateSavePolicy
            extractionStateSavePolicy = ExtractionStateSavePolicy.OnServerShutdown,
            StorageStateSavePolicy storageStateSavePolicy = StorageStateSavePolicy.OnServerShutdown) :
            base(dataConsumer, dataExtractor, upsertionCyclePeriod, upsertionStartTime,
                executionState)
        {
            _miscRepository = repository;
            _extractionStateSavePolicy = extractionStateSavePolicy;
            _storageStateSavePolicy = storageStateSavePolicy;
            
            if((extractionStateSavePolicy & ExtractionStateSavePolicy.OnChange) != 0)
            {
                dataExtractor.OnExecutionStateUpdate += DataExtractor_OnExecutionStateUpdate;
            }
            
            
        }

        private void DataExtractor_OnExecutionStateUpdate(CitilinkParsingExecutionState obj)
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
            var progress = _dataExtractor.GetProgress();
            SaveExtractionProgress(progress);
        }
        private void SaveExtractionProgress(CitilinkParsingExecutionState progress)
        {
            var progressDto = new CitilinkExtractionStateDto(progress.IsCompleted,
                progress.CurrentCatalogUrl, progress.CatalogPageNumber);

            ((IExtractionExecutionStateProvider<CitilinkExtractionStateDto>)_miscRepository).
                Save(progressDto);
            
        }
    }
}
