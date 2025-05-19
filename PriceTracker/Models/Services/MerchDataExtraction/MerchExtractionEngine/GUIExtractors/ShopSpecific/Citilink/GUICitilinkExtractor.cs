using PriceTracker.Models.DTOModels.ForParsing;
using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink.ExecutionState;
using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink.ExtractionInstructions;

namespace PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink
{
    public class GUICitilinkExtractor: IMerchDataExtractor<CitilinkMerchParsingDto, 
        CitilinkMerchExtractionInstruction>
    {
        // TODO [Arch]: Зарефакторить бы логику на этом и более нижнем уровне.
        private readonly CitilinkMerchParser _parser;
        private CitilinkMerchExtractionInstruction? _extractionData;


        public GUICitilinkExtractor(CitilinkScraper scraper, ILogger? logger = null) 
        {
            _parser = new(scraper, logger);
        }
        public async IAsyncEnumerable<CitilinkMerchParsingDto> 
            RunExtractionProcess(CitilinkMerchExtractionInstruction? extractionData = null)
        {
            _extractionData = extractionData;

            if(_extractionData == null)
            {
                _extractionData = new(new("", new(0)), false);

                await foreach (var merch in _parser.RetreiveAll(_extractionData.ExecutionState))
                    yield return merch;
            }
            else if(extractionData != null)
            {
                if (!extractionData.ExtractionProcessNew)
                    await foreach (var merch in
                        _parser.ContinueRetrieval(extractionData.ExecutionState))
                        yield return merch;
                else
                    await foreach(var merch in
                        _parser.RetreiveAll(extractionData.ExecutionState))
                        yield return merch;
            }
        }

        public CitilinkMerchExtractionInstruction? GetProgress()
        {
            return _extractionData;
        }

    }
}
