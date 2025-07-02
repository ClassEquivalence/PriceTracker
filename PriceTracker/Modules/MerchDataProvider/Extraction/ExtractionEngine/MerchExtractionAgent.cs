using PriceTracker.Modules.MerchDataProvider.MerchDataExtraction;
using PriceTracker.Modules.MerchDataProvider.MerchDataExtraction.ExecutionState;
using PriceTracker.Modules.MerchDataProvider.MerchDataExtraction.MerchExtractionEngine;
using PriceTracker.Modules.MerchDataProvider.Models.ForParsing;
using PriceTracker.Modules.Repository.Facade;

namespace PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine
{

    /// <summary>
    /// Не надо создавать или наследовать этот класс. Работать следует с обобщенным вариантом.
    /// </summary>
    public abstract class MerchExtractionAgent
    {
        public abstract Task StartNewExtraction();

        public abstract Task ContinueExtraction();
    }

    public class MerchExtractionAgent<Dto, ExtractionProcessInfo> : MerchExtractionAgent
        where Dto : MerchParsingDto where ExtractionProcessInfo : ExtractionExecutionStateInfo
    {

        public IMerchDataConsumer<Dto> Consumer;
        public IMerchDataExtractor<Dto, ExtractionProcessInfo> Extractor;
        private readonly IExtractionExecutionStateProvider<ExtractionProcessInfo>
            _stateProvider;
        private readonly ILogger? _logger;

        public MerchExtractionAgent(IMerchDataConsumer<Dto> consumer,
            IMerchDataExtractor<Dto, ExtractionProcessInfo> extractor,
            IExtractionExecutionStateProvider<ExtractionProcessInfo> stateProvider,
            ILogger? logger = null)
        {
            Consumer = consumer;
            Extractor = extractor;
            _stateProvider = stateProvider;
            Extractor.OnExecutionStateUpdate += _stateProvider.Save;
            _logger = logger;
        }

        public override async Task StartNewExtraction()
        {
            _logger?.LogTrace($"{nameof(MerchExtractionAgent)}: запускается" +
                $"извлечение товаров в {Consumer.GetType().Name}" +
                $"из {Extractor.GetType().Name}");
            await Consumer.ReceiveAsync(Extractor.RunExtractionProcess());
        }

        public override async Task ContinueExtraction()
        {
            _logger?.LogTrace($"{nameof(MerchExtractionAgent)}: продолжается" +
                $"извлечение товаров в {Consumer.GetType().Name}" +
                $"из {Extractor.GetType().Name}");
            await Consumer.ReceiveAsync(Extractor.
                RunExtractionProcess(_stateProvider.Provide()));
        }

    }
}
