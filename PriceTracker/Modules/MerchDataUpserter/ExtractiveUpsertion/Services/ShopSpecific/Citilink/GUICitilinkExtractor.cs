using Microsoft.Extensions.Logging;
using PriceTracker.Core.Configuration.ProvidedWithDI;
using PriceTracker.Core.Models.Process;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Mapping;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.MerchParser;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Utils.ScrapingServices.HttpClients.Browser;
using static PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.ICitilinkMerchCatalogUrlsParser;





namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink
{
    public class GUICitilinkExtractor : IMerchDataExtractor<CitilinkMerchParsingDto,
        CitilinkExtractionStateDto>
    {
        // TODO: Упорядочить завершение процесса (сейчас: процесс завершается как от
        // вызова IssueExtractionProcessHalt, так после проверки внутреннего состояния)
        private readonly CitilinkMerchParser _parser;
        private CitilinkMerchCatalogUrlsParser? _merchCatalogsParser;

        private bool isCompleted;
        private CitilinkCatalogUrlsTree _catalogUrlsTree;

        private CitilinkExtractionStateDto CitilinkExtractionState
        {
            get
            {
                CatalogUrlsTree tree = catalogUrlsTreeMapper.Map(_catalogUrlsTree);
                return new CitilinkExtractionStateDto(tree, isCompleted);
            }
        }

        private readonly ILogger? _logger;
        private readonly CitilinkScraper _baseScraper;
        private string? _storageState;
        private readonly CitilinkUpsertionOptions _upsertionOptions;

        private CitilinkCatalogUrlsTreeMapper catalogUrlsTreeMapper;


        public event Action<CitilinkExtractionStateDto>? OnExecutionStateUpdate;
        public event Action? ExtractionProcessFinished;
        private ExtractionPartialCycleResult extractionResultInfo;


        public GUICitilinkExtractor(BrowserAdapter browser, (int requests, TimeSpan period)
            maxPageRequestsPerTime, CitilinkCatalogUrlsTreeMapper catalogTreeMapper,
            CitilinkUpsertionOptions upsertionOptions,
            ILogger? logger = null, string? storageState = null)
        {
            CitilinkScraper baseScraper = _baseScraper = new(browser, 
                maxPageRequestsPerTime.requests, upsertionOptions, logger);

            baseScraper.RequestLimitReached += BaseScraper_OnRequestLimitReached;

            //CitilinkScraperSafeAccessAdapter scraper = new(baseScraper,
            //    maxPageRequestsPerTime);
            _parser = new CitilinkMerchParser(baseScraper, logger);


            _logger = logger;
            _storageState = storageState;
            _upsertionOptions = upsertionOptions;

            catalogUrlsTreeMapper = catalogTreeMapper;

            extractionResultInfo = ExtractionPartialCycleResult.CycleNotFinished;
        }


        public async IAsyncEnumerable<CitilinkMerchParsingDto>
            RunExtractionProcess()
        {
            isCompleted = false;
            await _baseScraper.PerformInitialRunupAsync(_storageState);

            _catalogUrlsTree = CreateInitialTree();

            _merchCatalogsParser = new(_baseScraper, _catalogUrlsTree, _upsertionOptions, _logger);

            _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}: начался новый процесс извлечения" +
                $" товаров.");


            var merches = ProcessExtraction();

            await foreach(var merch in merches)
            {
                yield return merch;
            }

        }

        public async IAsyncEnumerable<CitilinkMerchParsingDto>
            ContinueExtractionProcess(CitilinkExtractionStateDto extractionData)
        {
            isCompleted = false;
            await _baseScraper.PerformInitialRunupAsync(_storageState);

            _catalogUrlsTree = catalogUrlsTreeMapper.Map(extractionData.CachedUrls);

            _merchCatalogsParser = new(_baseScraper, _catalogUrlsTree, _upsertionOptions, _logger);

            _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}: инициировано продолжение процесса " +
                $"извлечения товаров.");

            _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(ContinueExtractionProcess)}:" +
                $" Roots children count: {extractionData.CachedUrls.Root.Children.Count()}");


            var merches = ProcessExtraction();
            await foreach (var merch in merches)
            {
                yield return merch;
            }

        }

        public void IssueExtractionProcessHalt(string reason)
        {
            extractionProcessHaltIssued = true;
            extractionProcessHaltReason = reason;
        }

        string extractionProcessHaltReason;
        bool extractionProcessHaltIssued = false;

        /// <summary>
        /// Метод прекращает работу, если:
        /// <br/>
        /// - Сервер начал посылать 429-ответы;
        /// <br/>
        /// - Достигнут лимит запросов, установленный в скрапере;
        /// <br/>
        /// - Полный цикл извлечения товаров завершен.
        /// </summary>
        /// <returns></returns>
        protected async IAsyncEnumerable<CitilinkMerchParsingDto> ProcessExtraction()
        {

            _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(ProcessExtraction)}: \n" +
                $"Извлечение товаров экстрактором Ситилинка началось.");

            List<BranchWithFunctionality>? branches;

            FunctionResult<List<BranchWithFunctionality>?, GetUrlsPortion_Info> getMerchCatalogUrlsResult;

            while (!extractionProcessHaltIssued && (branches = (getMerchCatalogUrlsResult = await _merchCatalogsParser.
                GetMerchCatalogUrlsPortion()).Result) != null && branches.Any())
            {
                _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(ProcessExtraction)}: \n" +
                    $"Главный цикл while ведёт работу.");

                if(getMerchCatalogUrlsResult.Info == GetUrlsPortion_Info.ServerTired)
                {
                    Stop_ServerTired(nameof(ProcessExtraction));
                    extractionResultInfo = ExtractionPartialCycleResult.HaltedAsTired;
                    yield break;
                }
                foreach (var branch in branches)
                {
                    _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(ProcessExtraction)}: \n" +
                    $"Цикл for в цикле while начал работу. for branch in branches (count={branches.Count}).");
                    if (extractionProcessHaltIssued)
                    {
                        break;
                    }
                    var retreiveResult = await _parser.RetreiveAllFromMerchCatalog(branch);
                    
                    var merches = retreiveResult.Result;

                    if (merches != null)
                    {
                        await foreach (var merch in merches)
                        {
                            yield return merch;
                        }
                        branch.IsProcessed = true;
                    }

                    if(retreiveResult.Info == CitilinkMerchParser.RetreiveAllFromMerchCatalog_ExecState.ServerGrownTired)
                    {
                        Stop_ServerTired(nameof(ProcessExtraction));
                        extractionResultInfo = ExtractionPartialCycleResult.HaltedAsTired;
                        yield break;
                    }
                    else if (retreiveResult.Info != CitilinkMerchParser.RetreiveAllFromMerchCatalog_ExecState
                        .Success)
                    {
                        OnExecutionStateUpdate?.Invoke(CitilinkExtractionState);
                        ExtractionProcessFinished?.Invoke();
                        _logger?.LogError($"{nameof(GUICitilinkExtractor)}, {nameof(ProcessExtraction)}: " +
                            $"Извлечение товаров остановлено: экстрактор более не способен" +
                            $" вытягивать товары по причине {retreiveResult.Info}");
                        extractionResultInfo = ExtractionPartialCycleResult.HaltedAsTired;
                        yield break;
                        
                    }

                    OnExecutionStateUpdate?.Invoke(CitilinkExtractionState);
                }
                if (extractionProcessHaltIssued)
                {
                    break;
                }
            }

            if (extractionProcessHaltIssued)
            {
                extractionProcessHaltIssued = false;
                _logger?.LogInformation($"{nameof(GUICitilinkExtractor)}, {nameof(RunExtractionProcess)}:" +
                    $" процесс извлечения данных о товарах завершается(приостанавливается) по причине:\n" +
                    $" {extractionProcessHaltReason}");

                OnExecutionStateUpdate?.Invoke(CitilinkExtractionState);
                ExtractionProcessFinished?.Invoke();

                extractionResultInfo = ExtractionPartialCycleResult.HaltedAsTired;

                yield break;
            }

            extractionResultInfo = ExtractionPartialCycleResult.HaltedAsFinished;
            OnExecutionStateUpdate?.Invoke(CitilinkExtractionState);
            ExtractionProcessFinished?.Invoke();
            _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(RunExtractionProcess)}:" +
            $" процесс извлечения данных о товарах из ситилинка завершился.");
        }


        private void BaseScraper_OnRequestLimitReached()
        {
            IssueExtractionProcessHalt("Достигнут лимит запросов скрапера.");
        }

        public CitilinkExtractionStateDto? GetProgress()
        {
            return CitilinkExtractionState;
        }

        public async Task<string> GetScraperStorageStateAsync()
        {
            return await _baseScraper.GetStorageStateAsync();
        }

        protected void Stop_ServerTired(string nameOfCaller)
        {
            OnExecutionStateUpdate?.Invoke(CitilinkExtractionState);
            ExtractionProcessFinished?.Invoke();
            _logger?.LogError($"{nameof(GUICitilinkExtractor)}, {nameOfCaller}: " +
                $"Извлечение товаров остановлено: экстрактор более не способен" +
                $" вытягивать товары: сервер устал(429).");
        }

        private CitilinkCatalogUrlsTree CreateInitialTree()
        {
            BranchWithFunctionality root = new(default, _upsertionOptions.CitilinkMainCatalogUrl, []);

            return new CitilinkCatalogUrlsTree(root);
        }

        public ExtractionPartialCycleResult GetResult()
        {
            return extractionResultInfo;
        }
    }
}

