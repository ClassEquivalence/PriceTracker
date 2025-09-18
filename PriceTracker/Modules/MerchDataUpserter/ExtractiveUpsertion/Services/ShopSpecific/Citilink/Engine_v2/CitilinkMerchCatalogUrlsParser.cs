using HtmlAgilityPack;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using static PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.ICitilinkMerchCatalogUrlsParser;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2
{
    /// <summary>
    /// TODO: убрать бы "магические константы" (особенно "notFound! !!!")
    /// </summary>
    public class CitilinkMerchCatalogUrlsParser : ICitilinkMerchCatalogUrlsParser
    {

        
        private readonly ILogger? _logger;


        private readonly ICitilinkScraper _scraper;

        private CitilinkCatalogUrlsTree catalogUrlsTree;

        private readonly CitilinkUpsertionOptions _options;

        public CitilinkCatalogUrlsTree CatalogUrlsTree
        {
            get => catalogUrlsTree;
            set
            {
                catalogUrlsTree = value;
                root = value.Root;
                currentBranchRoute = [];
                currentBranchRoute.Push(root);
                value.FiltersAndDuplicatesRemoved += OnCatalogUrlsTree_FiltersAndDuplicatesRemoved;
            }
        }

        private BranchWithFunctionality root;

        private Stack<BranchWithFunctionality> currentBranchRoute;

        private BranchWithFunctionality currentBranch => currentBranchRoute.Peek();


        public CitilinkMerchCatalogUrlsParser(ICitilinkScraper scraper,
            CitilinkCatalogUrlsTree catalogUrls, CitilinkUpsertionOptions options,
            ILogger? logger = null)
        {
            _options = options;
            CatalogUrlsTree = catalogUrls;
            _scraper = scraper;
            _logger = logger;
            root = catalogUrls.Root;
            currentBranchRoute = [];
            currentBranchRoute.Push(root);
        }


        /// <summary>
        /// Установить в качестве currentBranch наиболее молодую необработанную
        /// ветвь (но только из прямых предков текущей ветви).
        /// <br/>
        /// true - необработанная ветвь найдена.
        /// <br/>
        /// false - необработанных ветвей не осталось.
        /// </summary>
        /// <returns></returns>
        private bool TrySetCurrentBranch_YoungestUnprocessed()
        {
            while (currentBranch.IsProcessed)
            {
                if (currentBranchRoute.Any())
                {
                    currentBranchRoute.Pop();
                }
                else
                {
                    _logger?.LogInformation($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                        $"{nameof(TrySetCurrentBranch_YoungestUnprocessed)}: Найти необработанные ветви нельзя. " +
                        $" Они, похоже, закончились.");
                    return false;
                }
            }
            return true;
        }


        public async Task<FunctionResult<List<BranchWithFunctionality>?, GetUrlsPortion_Info>> 
            GetMerchCatalogUrlsPortion()
        {

            bool currentBranchUnprocessed = TrySetCurrentBranch_YoungestUnprocessed();
            if (!currentBranchUnprocessed)
                return new(null, GetUrlsPortion_Info.NoUnprocessedBranchesLeft);


            BranchFunctionality? functionality;

            bool merchBranchFound = false;

            while (!merchBranchFound)
            {
                var branchDataInsertionInfo = await EnsureDataLoadedToBranch(currentBranch);

                _logger?.LogTrace($"{nameof(CitilinkMerchCatalogUrlsParser)}, {nameof(GetMerchCatalogUrlsPortion)}: " +
                            $"{branchDataInsertionInfo}");

                switch (branchDataInsertionInfo)
                {
                    case BranchDataInsertionInfo.NotFound:
                        currentBranch.IsProcessed = true;
                        currentBranchRoute.Pop();
                        continue;

                    case BranchDataInsertionInfo.SeeminglyOk:
                        functionality = await TryGetPageFunctionality(currentBranch);

                        _logger?.LogTrace($"{nameof(CitilinkMerchCatalogUrlsParser)}, {nameof(GetMerchCatalogUrlsPortion)}: " +
                            $"{functionality}");

                        switch (functionality)
                        {
                            case BranchFunctionality.MerchCatalog:
                                merchBranchFound = true;
                                continue;

                            case BranchFunctionality.UnknownCatalogOfCatalogs:

                                var unprocessedChildren = await FindUnprocessedChildrenBranches(currentBranch);

                                if (unprocessedChildren == null)
                                {
                                    return new(null, GetUrlsPortion_Info.Error);
                                }

                                if (!unprocessedChildren.Any())
                                {
                                    currentBranch.IsProcessed = true;
                                    _logger?.LogDebug($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                                        $"{nameof(GetMerchCatalogUrlsPortion)}: Ветвь {currentBranch.Url} " +
                                        $"помечена, как обработанная");

                                    currentBranchUnprocessed = TrySetCurrentBranch_YoungestUnprocessed();
                                    if (!currentBranchUnprocessed)
                                        return new(null, GetUrlsPortion_Info.NoUnprocessedBranchesLeft);

                                }
                                else
                                {
                                    currentBranchRoute.Push(unprocessedChildren[0]);
                                }

                                continue;

                            case BranchFunctionality.SubCatalog:
                                goto case BranchFunctionality.UnknownCatalogOfCatalogs;

                            case BranchFunctionality.MainCatalog:
                                goto case BranchFunctionality.UnknownCatalogOfCatalogs;

                            default:
                                return new(null, GetUrlsPortion_Info.Error);
                        }

                    case BranchDataInsertionInfo.TooManyRequests:
                        return new(null, GetUrlsPortion_Info.ServerTired);

                    default:
                        return new(null, GetUrlsPortion_Info.Error);
                }

            }

            return new([currentBranch], GetUrlsPortion_Info.Success); 
        }


        /// <summary>
        /// null: Не удаётся найти из-за сервера
        /// пустой массив: их нет
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private async Task<List<BranchWithFunctionality>?> FindUnprocessedChildrenBranches(BranchWithFunctionality parent)
        {

            var branchDataInsertionInfo = await EnsureDataLoadedToBranch(parent);

            if (branchDataInsertionInfo != BranchDataInsertionInfo.SeeminglyOk)
            {
                return null;
            }

            List<BranchWithFunctionality> unprocessedChildren = [];

            CatalogUrlsTree.RemoveBranchFiltersAndDuplicates();

            foreach(var child in parent.Children)
            {
                if (!child.IsProcessed)
                {
                    unprocessedChildren.Add(child);
                }
            }

            return unprocessedChildren;
        }


        /// <summary>
        /// null возвращается, если установить функциональное назначение не удалось
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected async Task<BranchFunctionality?> TryGetPageFunctionality(BranchWithFunctionality catalog)
        {
            
            _logger?.LogTrace($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                $"{nameof(TryGetPageFunctionality)}: обрабатывается {catalog.Url} ");

            if (catalog.Url == _options.CitilinkMainCatalogUrl)
            {
                catalog.functionality = BranchFunctionality.MainCatalog;
                return BranchFunctionality.MainCatalog;
            }
            else if (catalog.Children.Any())
            {
                return BranchFunctionality.UnknownCatalogOfCatalogs;
            }
            else
            {
                var isLoaded = await EnsureDataLoadedToBranch(catalog);

                if (isLoaded!= BranchDataInsertionInfo.SeeminglyOk)
                {
                    return null;
                }

                var nullableFunctionality = catalog.functionality;
                if (nullableFunctionality == null)
                    throw new InvalidOperationException($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                        $"{nameof(TryGetPageFunctionality)}:\n Функциональное назначение страницы должно было" +
                        $" быть установлено ненулевым после {nameof(EnsureDataLoadedToBranch)}. Но оно - null.");
                BranchFunctionality functionality = nullableFunctionality ?? BranchFunctionality.Unknown;

                return functionality;
            }
        }



        protected BranchFunctionality ParsePageFunctionality(HtmlNode catalogNode)
        {

            //data-meta-name="CategoryCardsLayout" - для каталога с каталогами
            //data-meta-name="ProductListLayout" - для каталога с товарами

            var possibleNodeInPageOfCatalogs = catalogNode.SelectSingleNode
                ("//*[@data-meta-name=\"CategoryCardsLayout\"]");
            var possibleNodeInPageOfMerches = catalogNode.SelectSingleNode
                ("//*[@data-meta-name=\"ProductListLayout\"]");

            _logger?.LogTrace($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                $"{nameof(ParsePageFunctionality)}: \n" +
                $"possibleNodeInPageOfCatalogs = {possibleNodeInPageOfCatalogs},\n" +
                $"possibleNodeInPageOfMerches = {possibleNodeInPageOfMerches}");

            if (possibleNodeInPageOfCatalogs != null && possibleNodeInPageOfMerches == null)
            {
                return BranchFunctionality.SubCatalog;
            }
                
            else if (possibleNodeInPageOfMerches != null && possibleNodeInPageOfCatalogs == null)
            {
                return BranchFunctionality.MerchCatalog;
            }
                
            else
            {
                return BranchFunctionality.Unknown;
            }
                
        }






        /// <summary>
        /// Метод возвращает URL-адреса субкаталогов, содержащихся в другом субкаталоге.
        /// </summary>
        /// <param name="baseCatalogNode"></param>
        /// <returns>URL-адреса подкаталогов (содержащих либо товары, либо подкаталоги)</returns>
        private List<string> ParseSubCatalogsUrlsFromSub(HtmlNode baseCatalogNode, bool areUrlsAbsolute
            = false)
        {
            //data-meta-name="HolderPageLayout__aside"
            var asideNode = baseCatalogNode.SelectSingleNode("//*[@data-meta-name=\"HolderPageLayout__aside\"]");

            var anchorNodes = asideNode?.SelectNodes(".//a")?.ToList();
            if (anchorNodes == null)
                throw new InvalidOperationException("Не удалось спарсить узлы подкаталога.");

            List<string> urls = anchorNodes.Select(an => an.GetAttributeValue("href", "not found! !!!"))
                .ToList();

            if (urls.Any(url => url == "not found! !!!"))
                throw new InvalidOperationException("Не удалось спарсить ссылки из узлов подкаталога");

            if (areUrlsAbsolute)
            {
                urls = MakeUrlsAbsolute(urls);
            }

            return urls;
        }

        private List<string> ParseSubCatalogsUrlsFromMain(HtmlNode mainCatalogNode, bool areUrlsAbsolute =
            true)
        {
            var listNodes = mainCatalogNode.SelectNodes("//li[contains(@class, '-Item--StyledItem')]");
            List<HtmlNode?> nullableAnchorNodes;
            if (listNodes != null)
                nullableAnchorNodes = listNodes.Select(ln => ln.SelectSingleNode(".//a"))
                .ToList();
            else
                throw new InvalidOperationException("Не удалось запарсить подкаталоги: не запарсились" +
                    " ссылки соответствующих им узлов.");


            nullableAnchorNodes = nullableAnchorNodes.Where(an => an != null).ToList();
            List<HtmlNode> anchorNodes = nullableAnchorNodes.Select(an => an!).ToList();
            List<string> hrefs = anchorNodes.Select(an => an.GetAttributeValue("href", "notFound! !!!"))
                .ToList();
            if (hrefs.Any(s => s == "notFound! !!!"))
                throw new InvalidOperationException("Не удалось запарсить подкаталоги: не запарсились" +
                    "ссылки соответствующих им узлов.");

            if (areUrlsAbsolute)
            {
                hrefs = MakeUrlsAbsolute(hrefs);
            }

            return hrefs;
        }

        private List<string> MakeUrlsAbsolute(List<string> urls)
        {
            return urls.Select(
                u =>
                {
                    if (u.StartsWith(_options.CitilinkUrl))
                        return u;
                    else
                    {
                        return _options.CitilinkUrl.TrimEnd('/') + '/'
                        + u.TrimStart('/');
                    }
                }
                ).ToList();
        }



        private void OnCatalogUrlsTree_FiltersAndDuplicatesRemoved()
        {
            var all = CatalogUrlsTree.GetAllBranches();
            while (!all.Contains(currentBranch))
            {
                currentBranchRoute.Pop();
            }
        }


        public enum BranchDataInsertionInfo
        {
            SeeminglyOk,
            TooManyRequests,
            NotFound,
            ResponseError,
            ParsingError,
            UnknownError
        }

        /// <summary>
        /// Считать данные загруженными только в случае SeeminglyOk.
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        private async Task<BranchDataInsertionInfo> EnsureDataLoadedToBranch(BranchWithFunctionality branch)
        {

            if (branch.Children.Any())
            {
                if (branch.functionality != null)
                    return BranchDataInsertionInfo.SeeminglyOk;
                else
                {
                    branch.functionality = BranchFunctionality.UnknownCatalogOfCatalogs;
                    return BranchDataInsertionInfo.SeeminglyOk;
                }
            }
            else
            {
                // Загружаем html-документ ветви.
                var urlToNodeResult = await _scraper.UrlToNodeAsync(branch.Url);

                switch (urlToNodeResult.Info)
                {
                    case ICitilinkScraper.HtmlNodeRequestInfo.SeeminglyOk:

                        // Шаг 1: установка функционального назначения ветви.
                        var node = urlToNodeResult.Result;

                        BranchFunctionality functionality;
                        functionality = ParsePageFunctionality(node);

                        // Шаг 2: установка дочерних ветвей
                        if (functionality == BranchFunctionality.SubCatalog ||
                            functionality == BranchFunctionality.MainCatalog)
                        {
                            branch.functionality = functionality;
                            var allParsedChildren = ParseCatalogBranchDescendants(node, functionality);
                            var childrenWithoutIgnored = RemoveBranchesWithIgnoredCategorySlugs(allParsedChildren);
                            branch.Children = childrenWithoutIgnored;
                            return BranchDataInsertionInfo.SeeminglyOk;
                        }
                        else if (functionality == BranchFunctionality.MerchCatalog)
                        {
                            branch.functionality = functionality;
                            return BranchDataInsertionInfo.SeeminglyOk;
                        }
                        else
                        {
                            return BranchDataInsertionInfo.ParsingError;
                        }


                    case ICitilinkScraper.HtmlNodeRequestInfo.TooManyRequests:
                        return BranchDataInsertionInfo.TooManyRequests;
                        break;

                    case ICitilinkScraper.HtmlNodeRequestInfo.NotFound:
                        return BranchDataInsertionInfo.NotFound;
                        break;

                    case ICitilinkScraper.HtmlNodeRequestInfo.Error:
                        return BranchDataInsertionInfo.ResponseError;
                        break;

                    default:
                        return BranchDataInsertionInfo.UnknownError;
                }
            }
        }

        private List<BranchWithFunctionality> RemoveBranchesWithIgnoredCategorySlugs(List<BranchWithFunctionality> branches)
        {
            List<BranchWithFunctionality> branchesWithoutIgnoredCategorySlugs = [];
            foreach(var branch in branches)
            {
                if (!_options.IgnoredCategorySlugs.Contains(branch.GetCategorySlug()))
                    branchesWithoutIgnoredCategorySlugs.Add(branch);
            }
            return branchesWithoutIgnoredCategorySlugs;
        }

        private List<BranchWithFunctionality> ParseCatalogBranchDescendants(HtmlNode catalogNode,
            BranchFunctionality functionality)
        {

            switch (functionality)
            {
                
                case BranchFunctionality.MainCatalog:
                    List<string> urls = ParseSubCatalogsUrlsFromMain(catalogNode);
                    List<BranchWithFunctionality> subCatalogs = urls.Select(url =>
                    new BranchWithFunctionality(default, url, [], false)).ToList();
                    return subCatalogs;
                case BranchFunctionality.SubCatalog:
                    urls = ParseSubCatalogsUrlsFromSub(catalogNode);
                    subCatalogs = urls.Select(url =>
                    new BranchWithFunctionality(default, url, [], false)).ToList();
                    return subCatalogs;
                default:
                    throw new InvalidOperationException($"{nameof(ParseCatalogBranchDescendants)}:" +
                    $" Ошибка: нельзя парсить страницу с функциональностью {functionality}.");
            }

        }


    }
}
