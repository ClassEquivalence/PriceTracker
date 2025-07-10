using PriceTracker.Modules.MerchDataUpserter.Core.Models.ForParsing;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink.Engine;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Utils.ScrapingServices.HttpClients.Browser;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink
{
    public class GUICitilinkExtractor : IMerchDataExtractor<CitilinkMerchParsingDto,
        CitilinkParsingExecutionState>
    {
        // TODO [Arch]: Зарефакторить бы логику на этом и более нижнем уровне.
        private readonly CitilinkMerchParser _parser;
        private CitilinkParsingExecutionState? _extractionData;
        private readonly ILogger? _logger;

        public event Action<CitilinkParsingExecutionState>? OnExecutionStateUpdate;
        public event Action? ExtractionProcessFinished;

        public GUICitilinkExtractor(BrowserAdapter browser, (int requests, TimeSpan period)
            maxPageRequestsPerTime, ILogger? logger = null)
        {
            CitilinkScraper scraper = new(browser, logger, maxPageRequestsPerTime);
            _parser = new CitilinkMerchParser(scraper, logger);

            _logger = logger;

        }
        public async IAsyncEnumerable<CitilinkMerchParsingDto>
            RunExtractionProcess()
        {
            _extractionData = new("", 0, false);

            _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}: начался новый процесс извлечения" +
                $"товаров.");

            await foreach (var merch in _parser.RetreiveAll(_extractionData))
            {
                yield return merch;

                OnExecutionStateUpdate?.Invoke(_extractionData);
            }
            ExtractionProcessFinished?.Invoke();
        }

        public async IAsyncEnumerable<CitilinkMerchParsingDto>
            ContinueExtractionProcess(CitilinkParsingExecutionState extractionData)
        {
            _extractionData = extractionData;

            _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}: инициировано продолжение процесса " +
                $"извлечения товаров.");

            _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(RunExtractionProcess)}:" +
                $" extractionData != null");
            if (!_extractionData.IsCompleted)
            {
                _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(RunExtractionProcess)}:" +
                $" _extractionData.IsCompleted = false");
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
                $" _extractionData.IsCompleted = true");
                await foreach (var merch in
                    _parser.RetreiveAll(_extractionData))
                {
                    yield return merch;
                    OnExecutionStateUpdate?.Invoke(_extractionData);
                }
            }

            ExtractionProcessFinished?.Invoke();
            _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(RunExtractionProcess)}:" +
            $" процесс извлечения данных о товарах из ситилинка завершился.");



        }



        public CitilinkParsingExecutionState? GetProgress()
        {
            return _extractionData;
        }

    }
}
