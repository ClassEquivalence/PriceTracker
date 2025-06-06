using PriceTracker.Models.DTOModels.ForParsing;
using PriceTracker.Models.Services.MerchDataExtraction.ExecutionState;
using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine;
using PriceTracker.Modules.Repository.DataAccess.Repositories.Process;

namespace PriceTracker.Models.Services.MerchDataExtraction
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
        where Dto: MerchParsingDto where ExtractionProcessInfo: ExtractionExecutionStateInfo
    {

        public IMerchDataConsumer<Dto> Consumer;
        public IMerchDataExtractor<Dto, ExtractionProcessInfo> Extractor;
        private readonly IExtractionExecutionStateProvider<ExtractionProcessInfo>
            _stateProvider;

        public MerchExtractionAgent(IMerchDataConsumer<Dto> consumer,
            IMerchDataExtractor<Dto, ExtractionProcessInfo> extractor,
            IExtractionExecutionStateProvider<ExtractionProcessInfo> stateProvider) 
        {
            Consumer = consumer;
            Extractor = extractor;
            _stateProvider = stateProvider;
        }

        public override async Task StartNewExtraction()
        {
            await Consumer.ReceiveAsync(Extractor.RunExtractionProcess());
        }

        public override async Task ContinueExtraction()
        {
            await Consumer.ReceiveAsync(Extractor.
                RunExtractionProcess(_stateProvider.Provide()));
        }

    }
}
