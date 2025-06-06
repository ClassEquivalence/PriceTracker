using PriceTracker.Models.DataAccess.Repositories.Process;
using PriceTracker.Models.DTOModels.ForParsing;
using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink.ExtractionInstructions;

namespace PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink
{
    public class GUICitilinkExtractor: IMerchDataExtractor<CitilinkMerchParsingDto, 
        CitilinkParsingExecutionState>
    {
        // TODO [Arch]: Зарефакторить бы логику на этом и более нижнем уровне.
        private readonly CitilinkMerchParser _parser;
        private CitilinkParsingExecutionState? _extractionData;


        public GUICitilinkExtractor(CitilinkScraper scraper, 
            ILogger? logger = null) 
        {
            _parser = new(scraper, logger);
        }
        public async IAsyncEnumerable<CitilinkMerchParsingDto> 
            RunExtractionProcess(CitilinkParsingExecutionState? extractionData = null)
        {
            _extractionData = extractionData;

            if(_extractionData == null)
            {
                _extractionData = new("", 0, false);

                await foreach (var merch in _parser.RetreiveAll(_extractionData.ExecutionState))
                    yield return merch;
            }
            else if(extractionData != null)
            {
                if (!extractionData.IsResumed)
                    await foreach (var merch in
                        _parser.ContinueRetrieval(extractionData.ExecutionState))
                        yield return merch;
                else
                    await foreach(var merch in
                        _parser.RetreiveAll(extractionData.ExecutionState))
                        yield return merch;
            }
        }

        public CitilinkParsingExecutionState? GetProgress()
        {
            return _extractionData;
        }

    }
}
