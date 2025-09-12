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

        private BranchWithHtml root;

        private Stack<BranchWithHtml> currentBranchRoute;

        private BranchWithHtml currentBranch => currentBranchRoute.Peek();


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


        public async Task<FunctionResult<List<BranchWithHtml>?, GetUrlsPortion_Info>> 
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

            while((await GetPageFunctionality(currentBranch)) 
                != PageFunctionality.MerchCatalog)
            {

                if(await GetPageFunctionality(currentBranch) == PageFunctionality.ServerTired)
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
            /*

            if (await GetPageFunctionality(currentBranch) == PageFunctionality.MerchCatalog)
            {
                if (!(currentBranchRoute.Count > 1))
                {
                    throw new InvalidOperationException($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                        $"{nameof(GetMerchCatalogUrlsPortion)}: В стеке {nameof(currentBranchRoute)}" +
                        $" либо 1 либо 0 элементов. Но так быть не должно, когда текущая ветка " +
                        $"({currentBranch.Url}) отвечает за каталог товаров. То есть, не " +
                        $"может ветка каталога товаров быть корневой.");
                }

                currentBranchRoute.Pop();
                var unprocessedMerchCatalogChildren =
                    await FindUnprocessedMerchCatalogChildren(currentBranch);

                if (!unprocessedMerchCatalogChildren.Any())
                    throw new InvalidOperationException($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                        $"{nameof(GetMerchCatalogUrlsPortion)}: Противоречие: сперва у родительской ветви " +
                        $"({currentBranch.Url}) был" +
                        $" найден наследник, являющийся каталогом товаров. При этом же, у неё не найдено" +
                        $" ни одного наследника-каталога товаров.");

                return unprocessedMerchCatalogChildren;
            }
            else
            {
                throw new InvalidOperationException($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                        $"{nameof(GetMerchCatalogUrlsPortion)}: Ветвь ({currentBranch.Url})" +
                        $" при выходе из цикла должна была оказаться веткой товаров." +
                        $" При проверке через If, она ею не оказалась.");
            }

            */



        }


        /// <summary>
        /// null: Не удаётся найти из-за сервера
        /// пустой массив: их нет
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private async Task<List<BranchWithHtml>?> FindUnprocessedChildrenBranches(BranchWithHtml parent)
        {
            bool areLoaded = await LoadBranchDescendants(parent);

            if (!areLoaded)
            {
                return null;
            }

            List<BranchWithHtml> unprocessedChildren = [];

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
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private async Task<List<BranchWithHtml>> FindUnprocessedMerchCatalogChildren(BranchWithHtml parent)
        {
            List<BranchWithHtml> allUnprocessedChildren = 
                await FindUnprocessedChildrenBranches(parent);

            List<BranchWithHtml> merchCatalogUnprocessedChildren = [];

            foreach(var child in allUnprocessedChildren)
            {
                if (await GetPageFunctionality(child) == PageFunctionality.MerchCatalog)
                    merchCatalogUnprocessedChildren.Add(child);
            }
            return merchCatalogUnprocessedChildren;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private async Task<List<BranchWithHtml>> FindUnprocessedSubcatalogChildren(BranchWithHtml parent)
        {
            List<BranchWithHtml> allUnprocessedChildren =
                await FindUnprocessedChildrenBranches(parent);

            List<BranchWithHtml> subCatalogUnprocessedChildren = [];

            foreach (var child in allUnprocessedChildren)
            {
                var res = await GetPageFunctionality(child);
                if (res == PageFunctionality.MainCatalog
                || res == PageFunctionality.SubCatalog)
                    subCatalogUnprocessedChildren.Add(child);
            }
            return subCatalogUnprocessedChildren;
        }

        protected async Task<PageFunctionality> GetPageFunctionality(BranchWithHtml catalog)
        {
            
            _logger?.LogTrace($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                $"{nameof(GetPageFunctionality)}: обрабатывается {catalog.Url} ");

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
                await EnsureNodeInsertedToBranch(catalog);
                var functionality = GetPageFunctionality(catalog.Node!);
                catalog.functionality = functionality;
                return functionality;
            }
        }



        protected PageFunctionality GetPageFunctionality(HtmlNode catalogNode)
        {

            //data-meta-name="CategoryCardsLayout" - для каталога с каталогами
            //data-meta-name="ProductListLayout" - для каталога с товарами

            var possibleNodeInPageOfCatalogs = catalogNode.SelectSingleNode
                ("//*[@data-meta-name=\"CategoryCardsLayout\"]");
            var possibleNodeInPageOfMerches = catalogNode.SelectSingleNode
                ("//*[@data-meta-name=\"ProductListLayout\"]");

            _logger?.LogTrace($"{nameof(CitilinkMerchCatalogUrlsParser)}, " +
                $"{nameof(GetPageFunctionality)}: \n" +
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
        /// true: наследники либо уже загружены, либо загрузились
        /// при исполнении метода.
        /// <br/>
        /// false: наследники не могут быть загружены (по причине, например,
        /// 429-ответов сервера).
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private async Task<bool> LoadBranchDescendants(BranchWithHtml parent)
        {
            if (parent.Children != null && parent.Children.Any())
            {
                _logger?.LogTrace($"children==null? {parent.Children == null}\n" +
                    $"children.Any()? {parent.Children.Any()}");
                return true;
            }

            var functionality = await GetPageFunctionality(parent);
            _logger?.LogTrace($"{nameof(CitilinkMerchCatalogUrlsParser)}," +
                $" {nameof(LoadBranchDescendants)}: Функциональное назначение {parent.Url} - " +
                $"{functionality}");

            if(functionality == PageFunctionality.MainCatalog ||
                functionality == PageFunctionality.SubCatalog)
            {
                CatalogUrlsTree.RemoveBranchFiltersAndDuplicates();
                return await LoadCatalogBranchDescendants(parent);
            }
            else if(functionality == PageFunctionality.ServerTired)
            {
                return false;
            }
            else
            {

                parent.Children = [];
            }

            CatalogUrlsTree.RemoveBranchFiltersAndDuplicates();

            return true;

        }

        /// <summary>
        /// Иными словами, Find this catalog's subcatalogs.
        /// Метод ищет наследников данной ветки, если она:
        /// не имеет найденных наследников и является каталогом каталогов.
        /// В иных случаях - выбрасывает исключение.
        /// <br/>
        /// true: если удалось загрузить наследников
        /// false: если сервер "устал" и высылает ответы 429.
        /// </summary>
        /// <param name="parent"></param>
        private async Task<bool> LoadCatalogBranchDescendants(BranchWithHtml parent)
        {
            var functionality = await GetPageFunctionality(parent);

            _logger?.LogTrace($"{nameof(CitilinkMerchCatalogUrlsParser)}," +
                    $" {nameof(LoadCatalogBranchDescendants)}: Функциональное назначение" +
                    $" каталога {parent.Url} - {functionality}");

            if (parent.Children != null && parent.Children.Any())
            {
                throw new InvalidOperationException($"{nameof(LoadCatalogBranchDescendants)}:" + 
                    $" Ошибка: у ветви уже определены и найдены наследники.");
            }
            else if(functionality == PageFunctionality.MainCatalog)
            {
                _logger?.LogTrace($"{nameof(CitilinkMerchCatalogUrlsParser)}," +
                    $" {nameof(LoadCatalogBranchDescendants)}: сработала MainCatalog ветка.");
                await EnsureNodeInsertedToBranch(parent);

                List<string> urls = ParseSubCatalogsUrlsFromMain(parent.Node!);
                List<BranchWithHtml> subCatalogs = urls.Select(url => 
                new BranchWithHtml(default, url, [], false)).ToList();
                parent.Children = subCatalogs;

                _logger?.LogTrace($"{nameof(CitilinkMerchCatalogUrlsParser)}," +
                    $" {nameof(LoadCatalogBranchDescendants)}: subCatalogs count = " +
                    $"{subCatalogs.Count}");
            }
            else if(functionality == PageFunctionality.SubCatalog)
            {
                await EnsureNodeInsertedToBranch(parent);

                List<string> urls = ParseSubCatalogsUrlsFromSub(parent.Node!);
                List<BranchWithHtml> subCatalogs = urls.Select(url =>
                new BranchWithHtml(default, url, [], false)).ToList();

                parent.Children = subCatalogs;

            }
            else if(functionality == PageFunctionality.MerchCatalog)
            {
                throw new InvalidOperationException($"{nameof(LoadCatalogBranchDescendants)}:" +
                    $" Ошибка: у каталога товаров нет ветвей-наследников.");
            }
            else if(functionality == PageFunctionality.ServerTired)
            {
                return false;
            }
            else
            {
                throw new InvalidOperationException($"{nameof(LoadCatalogBranchDescendants)}:" +
                    $" Ошибка: не удается установить тип каталога.");
            }

            return true;
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

        /// <summary>
        /// true: Вставлен нужный узел
        /// <br/>
        /// false: Не вышло вставить узел (например, из-за ошибки 429)
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        private async Task<bool> EnsureNodeInsertedToBranch(BranchWithHtml branch)
        {
            if(branch.Node == null)
            {
                var urlToNodeResult = await _scraper.UrlToNodeAsync(branch.Url);
                if (urlToNodeResult.Info == ICitilinkScraper.HtmlNodeRequestInfo.SeeminglyOk)
                {
                    branch.Node = urlToNodeResult.Result;
                    return true;
                }
                else if (urlToNodeResult.Info == ICitilinkScraper.HtmlNodeRequestInfo.TooManyRequests)
                {
                    return false;
                }
                else
                    return false;
            }

            return true;
        }


        private void OnCatalogUrlsTree_FiltersAndDuplicatesRemoved()
        {
            var all = CatalogUrlsTree.GetAllBranches();
            while (!all.Contains(currentBranch))
            {
                currentBranchRoute.Pop();
            }
        }

    }
}
