using HtmlAgilityPack;
using PriceTracker.Core.Configuration.ProvidedWithDI;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using static PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.ICitilinkMerchCatalogUrlsParser;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2
{
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


        public async Task<FunctionResult<List<BranchWithFunctionality>?, GetUrlsPortion_Info>> 
            GetMerchCatalogUrlsPortion()
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
                        $"{nameof(GetMerchCatalogUrlsPortion)}: Найти необработанные ветви нельзя. " +
                        $" Они, похоже, закончились.");
                    return new(null, GetUrlsPortion_Info.NoUnprocessedUrlsLeft);
                }
            }

            PageFunctionality? functionality;

            while((functionality = await TryGetPageFunctionality(currentBranch)) 
                != PageFunctionality.MerchCatalog)
            {

                if(functionality == PageFunctionality.ServerTired ||
                    functionality == null)
                {
                    return new(null, GetUrlsPortion_Info.ServerTired);
                }

                var unprocessedChildren = await FindUnprocessedChildrenBranches(currentBranch);

                if(unprocessedChildren == null)
                {
                    return new(null, GetUrlsPortion_Info.ServerTired);
                }

                if (!currentBranch.Children.Any())
                {
                    throw new InvalidOperationException($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                        $"{nameof(GetMerchCatalogUrlsPortion)}: " +
                        $"У каталога {currentBranch.Url} нет наследников, хотя они должны быть, " +
                        $"учитывая что он не является каталогом товаров (а соответственно должен" +
                        $" быть каталогом каталогов).");
                }
                else if(!unprocessedChildren.Any())
                {
                    currentBranch.IsProcessed = true;

                    _logger?.LogDebug($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                        $"{nameof(GetMerchCatalogUrlsPortion)}: Ветвь {currentBranch.Url} " +
                        $"помечена, как обработанная");

                    if (!(currentBranchRoute.Count > 1))
                    {
                        _logger?.LogInformation($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                        $"{nameof(GetMerchCatalogUrlsPortion)}: Больше вернуть каталоги товаров нельзя." +
                        $" Они, похоже, закончились.");
                        return new(null, GetUrlsPortion_Info.NoUnprocessedUrlsLeft);

                    }
                    else
                    {
                        currentBranchRoute.Pop();
                        continue;
                    }
                }

                currentBranchRoute.Push(unprocessedChildren[0]);

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

            var isDataLoaded = await EnsureDataLoadedToBranch(parent);

            if (!isDataLoaded)
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
        protected async Task<PageFunctionality?> TryGetPageFunctionality(BranchWithFunctionality catalog)
        {
            
            _logger?.LogTrace($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                $"{nameof(TryGetPageFunctionality)}: обрабатывается {catalog.Url} ");

            if (catalog.Url == _options.CitilinkMainCatalogUrl)
            {
                catalog.functionality = PageFunctionality.MainCatalog;
                return PageFunctionality.MainCatalog;
            }
            else if (catalog.Children.Any())
            {
                return PageFunctionality.UnknownCatalogOfCatalogs;
            }
            else
            {
                var isLoaded = await EnsureDataLoadedToBranch(catalog);

                if (!isLoaded)
                {

                }

                var nullableFunctionality = catalog.functionality;
                if (nullableFunctionality == null)
                    throw new InvalidOperationException($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                        $"{nameof(TryGetPageFunctionality)}:\n Функциональное назначение страницы должно было" +
                        $" быть установлено ненулевым после {nameof(EnsureDataLoadedToBranch)}. Но оно - null.");
                PageFunctionality functionality = nullableFunctionality ?? PageFunctionality.Unknown;

                return functionality;
            }
        }



        protected PageFunctionality ParsePageFunctionality(HtmlNode catalogNode)
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
                return PageFunctionality.SubCatalog;
            }
                
            else if (possibleNodeInPageOfMerches != null && possibleNodeInPageOfCatalogs == null)
            {
                return PageFunctionality.MerchCatalog;
            }
                
            else
            {
                return PageFunctionality.Unknown;
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

        private async Task<bool> EnsureDataLoadedToBranch(BranchWithFunctionality branch)
        {

            if (branch.Children.Any())
            {
                if (branch.functionality != null)
                    return true;
                else
                    branch.functionality = PageFunctionality.UnknownCatalogOfCatalogs;
                return true;
            }
            else
            {
                // Загружаем html-документ ветви.
                var urlToNodeResult = await _scraper.UrlToNodeAsync(branch.Url);

                // Если нет ошибок при загрузке страницы - загружаем данные.
                if (urlToNodeResult.Info == ICitilinkScraper.HtmlNodeRequestInfo.SeeminglyOk)
                {
                    // Шаг 1: установка функционального назначения ветви.
                    var node = urlToNodeResult.Result;

                    PageFunctionality functionality;
                    branch.functionality = functionality = ParsePageFunctionality(node);

                    // Шаг 2: установка дочерних ветвей
                    if(functionality == PageFunctionality.SubCatalog ||
                        functionality == PageFunctionality.MainCatalog)
                    {
                        branch.Children = ParseCatalogBranchDescendants(node, functionality);
                    }

                    return true;
                }
                else if (urlToNodeResult.Info == ICitilinkScraper.HtmlNodeRequestInfo.TooManyRequests)
                {
                    _logger?.LogError($"{nameof(CitilinkMerchCatalogUrlsParser)}, {nameof(EnsureDataLoadedToBranch)}:\n" +
                        $" Сервер устал: не вышло загрузить данные в ветвь.");
                    return false;
                }
                else
                {
                    _logger?.LogError($"{nameof(CitilinkMerchCatalogUrlsParser)}, {nameof(EnsureDataLoadedToBranch)}:\n" +
                        $" Не вышло загрузить данные в ветвь по причине {urlToNodeResult.Info}");
                    return false;
                }
            }
        }



        private List<BranchWithFunctionality> ParseCatalogBranchDescendants(HtmlNode catalogNode,
            PageFunctionality functionality)
        {

            switch (functionality)
            {
                
                case PageFunctionality.MainCatalog:
                    List<string> urls = ParseSubCatalogsUrlsFromMain(catalogNode);
                    List<BranchWithFunctionality> subCatalogs = urls.Select(url =>
                    new BranchWithFunctionality(default, url, [], false)).ToList();
                    return subCatalogs;
                case PageFunctionality.SubCatalog:
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
