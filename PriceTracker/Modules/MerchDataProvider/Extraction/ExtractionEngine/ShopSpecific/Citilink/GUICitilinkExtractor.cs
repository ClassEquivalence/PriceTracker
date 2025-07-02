

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
        private readonly ILogger? _logger;

        public event Action<CitilinkParsingExecutionState>? OnExecutionStateUpdate;

        public GUICitilinkExtractor(CitilinkMerchParser parser,
            ILogger? logger = null)
        {
            _parser = parser;
            _logger = logger;

        }
        public async IAsyncEnumerable<CitilinkMerchParsingDto>
            RunExtractionProcess(CitilinkParsingExecutionState? extractionData = null)
        {
            _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(RunExtractionProcess)}:" +
                $" идёт процесс извлечения данных о товарах из ситилинка.");
            _extractionData = extractionData;

            if (_extractionData == null)
            {
                _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(RunExtractionProcess)}:" +
                $" extractionData = null");
                _extractionData = new("", 0, false);

                await foreach (var merch in _parser.RetreiveAll(_extractionData))
                {
                    yield return merch;

                    OnExecutionStateUpdate?.Invoke(_extractionData);
                }
            }
            else if (_extractionData != null)
            {
                _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(RunExtractionProcess)}:" +
                $" extractionData != null");
                if (_extractionData.IsResumed)
                {
                    _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(RunExtractionProcess)}:" +
                    $" _extractionData.IsResumed = true");
                    await foreach (var merch in
                        _parser.ContinueRetrieval(_extractionData))
                    {
                        yield return merch;
                        OnExecutionStateUpdate?.Invoke(_extractionData);
                    }
                }
                else
                {
                    _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(RunExtractionProcess)}:" +
                    $" _extractionData.IsResumed = false");
                    await foreach (var merch in
                        _parser.RetreiveAll(_extractionData))
                    {
                        yield return merch;
                        OnExecutionStateUpdate?.Invoke(_extractionData);
                    }
                }
                _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(RunExtractionProcess)}:" +
                $" процесс извлечения данных о товарах из ситилинка завершился.");
            }
        }

        public CitilinkParsingExecutionState? GetProgress()
        {
            return _extractionData;
        }

    }
}
