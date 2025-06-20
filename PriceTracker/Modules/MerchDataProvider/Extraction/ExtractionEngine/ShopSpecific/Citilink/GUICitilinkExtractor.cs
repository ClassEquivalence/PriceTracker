

using PriceTracker.Modules.MerchDataProvider.MerchDataExtraction.MerchExtractionEngine;
using PriceTracker.Modules.MerchDataProvider.Models.ForParsing;

namespace PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine.ShopSpecific.Citilink
{
    public class GUICitilinkExtractor : IMerchDataExtractor<CitilinkMerchParsingDto,
        CitilinkParsingExecutionState>
    {
        // TODO [Arch]: Зарефакторить бы логику на этом и более нижнем уровне.
        private readonly CitilinkMerchParser _parser;
        private CitilinkParsingExecutionState? _extractionData;

        public event Action<CitilinkParsingExecutionState>? OnExecutionStateUpdate;

        public GUICitilinkExtractor(CitilinkScraper scraper,
            ILogger? logger = null)
        {
            _parser = new(scraper, logger);
            
        }
        public async IAsyncEnumerable<CitilinkMerchParsingDto>
            RunExtractionProcess(CitilinkParsingExecutionState? extractionData = null)
        {
            _extractionData = extractionData;

            if (_extractionData == null)
            {
                _extractionData = new("", 0, false);

                await foreach (var merch in _parser.RetreiveAll(_extractionData))
                {
                    yield return merch;
                    
                    OnExecutionStateUpdate?.Invoke(_extractionData);
                }
            }
            else if (_extractionData != null)
            {
                if (!_extractionData.IsResumed)
                    await foreach (var merch in
                        _parser.ContinueRetrieval(_extractionData))
                    {
                        yield return merch;
                        OnExecutionStateUpdate?.Invoke(_extractionData);
                    }
                else
                    await foreach (var merch in
                        _parser.RetreiveAll(_extractionData))
                    {
                        yield return merch;
                        OnExecutionStateUpdate?.Invoke(_extractionData);
                    }
            }
        }

        public CitilinkParsingExecutionState? GetProgress()
        {
            return _extractionData;
        }

    }
}
