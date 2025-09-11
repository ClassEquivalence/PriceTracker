using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
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
            DateTime upsertionStartTime, 
            ICitilinkMiscellaneousRepositoryFacade repository, ILogger? logger, TimeSpan
            upsertionRestPeriod, ExtractionStateSavePolicy
            extractionStateSavePolicy = ExtractionStateSavePolicy.OnServerShutdown,
            StorageStateSavePolicy storageStateSavePolicy = StorageStateSavePolicy.OnServerShutdown) :
            base(dataConsumer, dataExtractor, upsertionCyclePeriod, upsertionRestPeriod, upsertionStartTime,
                logger)
        {
            _miscRepository = repository;
            _extractionStateSavePolicy = extractionStateSavePolicy;
            _storageStateSavePolicy = storageStateSavePolicy;

            if ((extractionStateSavePolicy & ExtractionStateSavePolicy.OnChange) != 0)
            {
                dataConsumer.MerchPortionUpserted += () =>
                {
                    UpdateUpsertionProgress();
                    DataConsumer_OnMerchPortionUpserted();
                };
            }
            else
            {
                dataConsumer.MerchPortionUpserted += UpdateUpsertionProgress;
            }


        }

        public override async Task ProcessUpsertion()
        {
            //await base.ProcessUpsertion();
           
            try
            {
                await base.ProcessUpsertion();
            }
            catch(Exception ex)
            {
                _logger?.LogError($"Апсершн ситилинка прекращен из-за исключения: {ex.Message}");

                
                SaveExtractionProgress();
                await SaveStorageState();

                throw new InvalidOperationException($"Апсершн ситилинка прекращен из-за исключения: {ex.Message}",
                    ex);
            }

            var saveStorageTask = SaveStorageState();
            
            SaveExtractionProgress();
            await saveStorageTask;
        }



        /// <summary>
        /// Апдейт и сейв прогресса апсершна разделены, потому что
        /// апсершн происходит отдельно от извлечения, и апдейт можно вызывать
        /// только удостоверившись что нужные данные достигли апсершна.
        /// </summary>
        private void UpdateUpsertionProgress()
        {
            _executionState = _dataExtractor.GetProgress().DeepClone();
        }

        private void DataConsumer_OnMerchPortionUpserted()
        {
            SaveExtractionProgress();
        }

        private void DataExtractor_OnExecutionStateUpdate(CitilinkExtractionStateDto obj)
        {
            SaveExtractionProgress(obj);
        }

        public override async Task OnShutDownAsync()
        {
            //var baseShutDownTask = base.OnShutDown();

            if ((_extractionStateSavePolicy & ExtractionStateSavePolicy.OnServerShutdown) != 0)
            {
                _logger?.LogDebug($"{nameof(ScheduledCitilinkMerchUpserter)}, {nameof(OnShutDownAsync)}:\n" +
                    $"Состояние извлечения сохраняется из за приостановки работы приложения.");
                //UpdateExtractionProgress();
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
            if (_executionState == null)
                throw new InvalidOperationException($"{nameof(ScheduledCitilinkMerchUpserter)}, {nameof(SaveExtractionProgress)}:\n" +
                    $"{nameof(_executionState)} == null");
            SaveExtractionProgress(_executionState);
        }
        private void SaveExtractionProgress(CitilinkExtractionStateDto progress)
        {

            ((IExtractionExecutionStateProvider<CitilinkExtractionStateDto>)_miscRepository).
                Save(progress);

        }

        public override CitilinkExtractionStateDto? TryLoadExecutionState()
        {
            return _miscRepository.Provide();
        }

        public override CitilinkExtractionStateDto CreateNewExecutionState()
        {
            return new(null, true);
        }
    }
}
