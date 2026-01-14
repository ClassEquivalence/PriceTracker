using PriceTracker.Core.Configuration.ProvidedWithDI;
using PriceTracker.Core.Models.Process;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ParsingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.UpsertionLayer;
using PriceTracker.Modules.Repository.Facade.Citilink;
using PriceTracker.Modules.Repository.Facade.FacadeInterfaces;
using System.Threading.Tasks;
using static PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.UpsertionLayer.IMerchUpsertionProcessor;
using static PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.ScheduledCitilinkMerchUpserter;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionLayer
{
    public class CitilinkMerchUpsertionProcessor: IMerchUpsertionProcessor
    {

        protected readonly ILogger? _logger;

        private readonly IMerchDataConsumer<CitilinkMerchParsingDto> _dataConsumer;
        private readonly IMerchDataExtractor<CitilinkMerchParsingDto, CitilinkExtractionStateDto>
            _dataExtractor;

        private readonly ICitilinkMiscellaneousRepositoryFacade _miscRepository;

        private readonly ExtractionStateSavePolicy _extractionStateSavePolicy;

        private CitilinkExtractionStateDto? _executionState;

        [Flags]
        public enum ExtractionStateSavePolicy
        {
            None = 0,
            OnUpsertionHaltIssued = 1 << 0,
        }

        public CitilinkMerchUpsertionProcessor(CitilinkMerchDataUpserter
            dataConsumer, GUICitilinkExtractor dataExtractor,
            ICitilinkMiscellaneousRepositoryFacade repository,
            ExtractionStateSavePolicy
            extractionStateSavePolicy = ExtractionStateSavePolicy.OnUpsertionHaltIssued)
        {
            _dataConsumer = dataConsumer;
            _dataExtractor = dataExtractor;
            _extractionStateSavePolicy = extractionStateSavePolicy;
            _miscRepository = repository;
            dataConsumer.MerchPortionUpserted += UpdateUpsertionProgress;
        }

        public async Task<UpsertionSubcycleResult> ProcessUpsertionSubCycleAsync()
        {
            if (_executionState == null)
            {
                _executionState = GetOrCreateExecState();
            }

            if (_executionState.IsCompleted)
            {
                await _dataConsumer.Upsert(_dataExtractor.
                    RunExtractionProcess());
            }
            else
            {
                await _dataConsumer.Upsert(_dataExtractor.
                    ContinueExtractionProcess());
            }

            // TODO: Костыль, т.к. возврат сокращает информацию и признаёт
            // подцикл завершенным, не вдаваясь в причины завершения.
            return _dataExtractor.GetResult() switch
            {
                ExtractionPartialCycleResult.HaltedAsFinished =>
                UpsertionSubcycleResult.EntireCycleCompleted,
                _ => UpsertionSubcycleResult.SubcycleCompleted
            };

        }


        /// <summary>
        /// ВАЖНО: Save и Update должны вызываться так, чтобы сохранялась связность 
        /// экстрактора и консюмера.
        /// <br/>
        /// Апдейт и сейв прогресса апсершна разделены, потому что
        /// апсершн происходит отдельно от извлечения, и апдейт можно вызывать
        /// только удостоверившись что нужные данные достигли апсершна.
        /// </summary>
        private void UpdateUpsertionProgress()
        {
            _executionState = _dataExtractor.GetProgress().DeepClone();
        }

        public async Task OnProcessHalt()
        {
            //var baseShutDownTask = base.OnShutDown();

            if ((_extractionStateSavePolicy & ExtractionStateSavePolicy.OnUpsertionHaltIssued) != 0)
            {
                _logger?.LogDebug($"{nameof(ScheduledCitilinkMerchUpserter)}, {nameof(OnProcessHalt)}:\n" +
                    $"Состояние извлечения сохраняется из за приостановки работы приложения.");
                //UpdateExtractionProgress();
                SaveExtractionProgress();
            }
        }

        /// <summary>
        /// ВАЖНО: Save и Update должны вызываться так, чтобы сохранялась связность 
        /// экстрактора и консюмера.
        /// <br/>
        /// Апдейт и сейв прогресса апсершна разделены, потому что
        /// апсершн происходит отдельно от извлечения, и апдейт можно вызывать
        /// только удостоверившись что нужные данные достигли апсершна.
        /// </summary>
        public void SaveExtractionProgress()
        {
            if (_executionState == null)
                throw new InvalidOperationException($"{nameof(ScheduledCitilinkMerchUpserter)}, {nameof(SaveExtractionProgress)}:\n" +
                    $"{nameof(_executionState)} == null");
            SaveExtractionProgress(_executionState);
        }

        /// <summary>
        /// ВАЖНО: Save и Update должны вызываться так, чтобы сохранялась связность 
        /// экстрактора и консюмера.
        /// <br/>
        /// Апдейт и сейв прогресса апсершна разделены, потому что
        /// апсершн происходит отдельно от извлечения, и апдейт можно вызывать
        /// только удостоверившись что нужные данные достигли апсершна.
        /// </summary>
        private void SaveExtractionProgress(CitilinkExtractionStateDto progress)
        {
            _miscRepository.Save(progress);
        }

        public CitilinkExtractionStateDto GetOrCreateExecState()
        {
            return TryLoadExecutionState() ?? CreateNewExecutionState();
        }

        public CitilinkExtractionStateDto? TryLoadExecutionState()
        {
            return _miscRepository.Provide();
        }

        public CitilinkExtractionStateDto CreateNewExecutionState()
        {
            return new(null, true);
        }
    }
}
