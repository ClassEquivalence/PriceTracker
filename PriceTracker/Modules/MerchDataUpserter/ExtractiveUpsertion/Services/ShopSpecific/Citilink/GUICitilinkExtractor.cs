using Microsoft.Extensions.Logging;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
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
        private readonly CitilinkUpsertionOptions _upsertionOptions;

        private CitilinkCatalogUrlsTreeMapper catalogUrlsTreeMapper;


        public event Action<CitilinkExtractionStateDto>? OnExecutionStateUpdate;
        public event Action? ExtractionProcessFinished;
        private ExtractionPartialCycleResult extractionResultInfo;


        public GUICitilinkExtractor(CitilinkCatalogUrlsTreeMapper catalogTreeMapper,
            CitilinkUpsertionOptions upsertionOptions, string userAgent,
            ILogger? logger = null)
        {
            CitilinkScraper baseScraper = _baseScraper = new(upsertionOptions, userAgent, logger);

            baseScraper.RequestLimitReached += BaseScraper_OnRequestLimitReached;

            _parser = new CitilinkMerchParser(baseScraper, logger,
                upsertionOptions.IgnoredCategorySlugs.ToList());


            _logger = logger;
            _upsertionOptions = upsertionOptions;

            catalogUrlsTreeMapper = catalogTreeMapper;

            extractionResultInfo = ExtractionPartialCycleResult.CycleNotFinished;
        }


        public async IAsyncEnumerable<CitilinkMerchParsingDto>
            RunExtractionProcess()
        {
            isCompleted = false;

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
        /// <br/>
        /// - После вызова IssueExtractionProcessHalt
        /// </summary>
        /// <returns></returns>
        protected async IAsyncEnumerable<CitilinkMerchParsingDto> ProcessExtraction()
        {

            _logger?.LogTrace($"{nameof(GUICitilinkExtractor)}, {nameof(ProcessExtraction)}: \n" +
                $"Извлечение товаров экстрактором Ситилинка началось.");

            List<BranchWithFunctionality>? branches;

            FunctionResult<List<BranchWithFunctionality>?, GetUrlsPortion_Info> getMerchCatalogUrlsResult;

            while (!extractionProcessHaltIssued)
            {
                // Этап 1: получение нужного URL-адреса каталога товаров.
                getMerchCatalogUrlsResult = await _merchCatalogsParser.GetMerchCatalogUrlsPortion();
                branches = getMerchCatalogUrlsResult.Result;
                if (!HandleUrlPortionResult(getMerchCatalogUrlsResult.Info))
                    yield break;

                // Проверка наличия ветвей в возвращаемом результате.
                if (branches == null || !branches.Any())
                {
                    throw new InvalidOperationException($"{nameof(GUICitilinkExtractor)}, {nameof(ProcessExtraction)}: " +
                        $" Неожиданное поведение: ветвей из {nameof(_merchCatalogsParser.GetMerchCatalogUrlsPortion)} " +
                        $"при успешном результате должно быть больше 0.");
                }

                // Этап 2: Извлечение товаров по URL-адресу каталога товаров.
                foreach (var branch in branches)
                {
                    if (extractionProcessHaltIssued)
                    {
                        break;
                    }
                    var merchResult = await _parser.RetreiveAllFromMerchCatalog(branch);

                    switch (merchResult.Info)
                    {
                        case CitilinkMerchParser.RetreiveAllFromMerchCatalog_ExecState.Success:
                            var merches = merchResult.Result;
                            if (merches != null)
                            {
                                await foreach (var merch in merches)
                                {
                                    yield return merch;
                                }
                                branch.IsProcessed = true;
                            }
                            break;

                        case CitilinkMerchParser.RetreiveAllFromMerchCatalog_ExecState.ServerGrownTired:
                            Stop_ServerTired(nameof(ProcessExtraction));
                            yield break;

                        case CitilinkMerchParser.RetreiveAllFromMerchCatalog_ExecState.PassedIgnoredCategorySlug:
                            branch.IsProcessed = true;
                            break;

                        default:
                        case CitilinkMerchParser.RetreiveAllFromMerchCatalog_ExecState.UnknownServerError:
                            Stop_UnknownError(nameof(ProcessExtraction));
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
                Stop_ProcessHaltIssued(nameof(ProcessExtraction));
                yield break;
            }

        }


        private bool HandleUrlPortionResult(GetUrlsPortion_Info info)
        {
            switch (info)
            {
                case GetUrlsPortion_Info.Success:
                    return true;
                case GetUrlsPortion_Info.ServerTired:
                    Stop_ServerTired(nameof(ProcessExtraction));
                    return false;
                case GetUrlsPortion_Info.NoUnprocessedBranchesLeft:
                    Stop_NoUnprocessedBranchesLeft(nameof(ProcessExtraction));
                    return false;
                case GetUrlsPortion_Info.Error:
                default:
                    Stop_UnknownError(nameof(ProcessExtraction));
                    return false;
            }
        }

        
        private void BaseScraper_OnRequestLimitReached()
        {
            IssueExtractionProcessHalt("Достигнут лимит запросов скрапера.");
            _baseScraper.RefreshRequestsCount();
        }

        public CitilinkExtractionStateDto? GetProgress()
        {
            return CitilinkExtractionState;
        }

        protected void Stop_ServerTired(string nameOfCaller)
        {
            extractionResultInfo = ExtractionPartialCycleResult.HaltedAsTired;
            OnExecutionStateUpdate?.Invoke(CitilinkExtractionState);
            ExtractionProcessFinished?.Invoke();
            _logger?.LogError($"{nameof(GUICitilinkExtractor)}, {nameOfCaller}: " +
                $"Извлечение товаров остановлено: экстрактор более не способен" +
                $" вытягивать товары: сервер устал(429).");
        }

        protected void Stop_UnknownError(string nameOfCaller)
        {
            extractionResultInfo = ExtractionPartialCycleResult.UnknownError;
            OnExecutionStateUpdate?.Invoke(CitilinkExtractionState);
            ExtractionProcessFinished?.Invoke();
            _logger?.LogError($"{nameof(GUICitilinkExtractor)}, {nameOfCaller}: " +
                $"Извлечение товаров остановлено: экстрактор более не способен" +
                $" вытягивать товары: неизвестная ошибка.");
        }

        protected void Stop_NoUnprocessedBranchesLeft(string nameOfCaller)
        {
            extractionResultInfo = ExtractionPartialCycleResult.HaltedAsFinished;
            CitilinkExtractionState.IsCompleted = true;
            OnExecutionStateUpdate?.Invoke(CitilinkExtractionState);
            ExtractionProcessFinished?.Invoke();
            _logger?.LogInformation($"{nameof(GUICitilinkExtractor)}, {nameOfCaller}:" +
            $" процесс извлечения данных о товарах из ситилинка успешно завершился.");
        }

        protected void Stop_ProcessHaltIssued(string nameOfCaller)
        {
            extractionResultInfo = ExtractionPartialCycleResult.HaltedAsTired;
            extractionProcessHaltIssued = false;
            OnExecutionStateUpdate?.Invoke(CitilinkExtractionState);
            ExtractionProcessFinished?.Invoke();
            _logger?.LogInformation($"{nameof(GUICitilinkExtractor)}, {nameof(ProcessExtraction)}:" +
                $" процесс извлечения данных о товарах завершается(приостанавливается) по причине:\n" +
                $" {extractionProcessHaltReason}");
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

