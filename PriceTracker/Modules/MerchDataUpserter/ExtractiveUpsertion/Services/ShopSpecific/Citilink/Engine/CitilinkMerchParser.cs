using HtmlAgilityPack;
using PriceTracker.Core.Models.Process.ShopSpecific.Citilink;
using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataUpserter.Core.Models.ForParsing;









// TODO: ТУТ ОЧЕНЬ МНОГО ВСЕГО ПОМЕНЯТЬ И ПРИЧЕСАТЬ. И ПРОТЕСТИРОВАТЬ.

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink.Engine
{
    public class CitilinkMerchParser
    {

        const string merchCatalogPaginationQueryString = "?ref=mainmenu&p=";
        const string baseUrl = "https://www.citilink.ru";

        private readonly ILogger? _logger;


        private readonly ICitilinkScraper _scraper;

        public CitilinkMerchParser(ICitilinkScraper scraper, ILogger? logger = null)
        {
            // TODO: оптимизировать до нормальной асинхронности можно.
            _scraper = scraper;
            _logger = logger;
        }

        public async IAsyncEnumerable<CitilinkMerchParsingDto> RetreiveAll(CitilinkExtractionStateDto
            executionState)
        {
            await foreach (var dto in ParseAll(executionState, false))
            {
                yield return dto;
            }
        }

        public async IAsyncEnumerable<CitilinkMerchParsingDto> ContinueRetrieval(CitilinkExtractionStateDto executionState)
        {
            await foreach (var dto in ParseAll(executionState, true))
            {
                yield return dto;
            }
        }

        protected async IAsyncEnumerable<CitilinkMerchParsingDto> ParseAll(CitilinkExtractionStateDto execState,
            bool continueFromExecState = false)
        {
            _logger?.LogTrace($"{nameof(CitilinkMerchParser)}, {nameof(ParseAll)}: " +
                $"начался парсинг-процесс.");
            var catalogUrls = RetrieveAllMerchCatalogsUrls();
            bool hasExtractionContinued = false;

            if (string.IsNullOrWhiteSpace(execState.CurrentCatalogUrl))
                continueFromExecState = false;

            await foreach (var url in catalogUrls)
            {

                if (!hasExtractionContinued && continueFromExecState)
                {
                    if (url.TrimEnd('/') != execState.CurrentCatalogUrl.TrimEnd('/'))
                    {
                        _logger?.LogTrace($"Пропускаем {url}, ждём, когда попадётся " +
                            $"{execState.CurrentCatalogUrl}");
                        continue;
                    }
                    else
                    {
                        _logger?.LogInformation($"Извлечение товаров продолжается с {url}");
                    }
                }



                execState = execState with { CurrentCatalogUrl = url };

                // Если процесс продолжается с прошлой остановки, продолжить со страницы остановки.
                // Иначе - действовать сначала.
                bool continueFromPage = continueFromExecState && !hasExtractionContinued;

                _logger?.LogTrace($"{nameof(CitilinkMerchParser)}, {nameof(ParseAll)}: " +
                    $"должно произойти извлечение товаров по url " +
                    $"{url}");

                var catalogMerches = RetrieveMerchesFromCatalog(url, execState,
                    continueFromPage);
                await foreach (var dto in catalogMerches)
                    yield return dto;

                hasExtractionContinued = true;
            }


            execState = execState with { IsCompleted = true };

        }

        public async IAsyncEnumerable<CitilinkMerchParsingDto> RetrieveMerchesFromCatalog(string catalogUrl,
            CitilinkExtractionStateDto execState, bool continueFromExecState = false)
        {
            _logger?.LogDebug($"{nameof(RetrieveMerchesFromCatalog)}: начато извлечение товаров из каталога {catalogUrl}");

            int numberOfPages = ParsePageCount(await _scraper.UrlToNode(catalogUrl));
            var urlWithoutQuery = catalogUrl.SubstringBeforeFirstEntryOrEmpty("?");

            string urlWithQueryWithoutPage = urlWithoutQuery + merchCatalogPaginationQueryString; // + int page;

            int i = 1;
            if (continueFromExecState)
                i = execState.CatalogPageNumber > 0 ? execState.CatalogPageNumber : 1;

            for (; i <= numberOfPages; i++)
            {
                foreach (var dto in await ParsePortionFromUrl(urlWithQueryWithoutPage + i))
                {
                    yield return dto;
                }

                execState = execState with { CatalogPageNumber = i };
            }
        }


        public async IAsyncEnumerable<string> RetrieveAllMerchCatalogsUrls()
        {
            _logger?.LogTrace($"{nameof(CitilinkMerchParser)} {nameof(RetrieveAllMerchCatalogsUrls)}:" +
                $"Вытягивание информации о всех каталогах товаров начато.");

            var mainCatalogSectionsNode = await _scraper.UrlToNode("https://www.citilink.ru/catalog/");



            async IAsyncEnumerable<string> RecursiveMerchCatalogRetreive(string subCatalogRelativeUrl)
            {
                string subCatalogUrl = baseUrl + subCatalogRelativeUrl.
                    SubstringWithAndAfterFirstEntryOrEmpty("/catalog");
                _logger?.LogTrace($"{nameof(RecursiveMerchCatalogRetreive)}: " +
                    $"Сформирован URL: {subCatalogUrl}");

                var subCatalogNode = await _scraper.UrlToNode(subCatalogUrl);
                if (IsCatalogForMerches(subCatalogNode))
                    yield return subCatalogUrl;
                else
                {
                    List<string> RetreivedMerchCatalogUrls = [];
                    var subcatalogs = ParseSubCatalogsUrlsFromSub(subCatalogNode);
                    foreach (string subCatalog in subcatalogs)
                    {
                        if (subCatalog.Contains("kompyutery--konfigurator") ||
                            subCatalog.Contains("congigurator"))
                            continue;

                        var subSubCatalogs = RecursiveMerchCatalogRetreive(subCatalog);
                        await foreach (string subSubCatalog in subSubCatalogs)
                        {
                            yield return subSubCatalog;
                        }
                    }
                }
            }

            List<string> subCatalogUrls = ParseSubCatalogsUrlsFromMain(mainCatalogSectionsNode);
            List<string> merchCatalogs = [];

            foreach (string url in subCatalogUrls)
            {
                var subSubCatalogUrls = RecursiveMerchCatalogRetreive(url);
                await foreach (string subUrl in subSubCatalogUrls)
                {
                    yield return subUrl;
                }
            }
            _logger?.LogTrace($"{nameof(CitilinkMerchParser)} {nameof(RetrieveAllMerchCatalogsUrls)}:" +
                $"Вытягивание информации о всех каталогах товаров завершено.");
        }


        /// <summary>
        /// Определяет, предложенный каталог является каталогом, содержащим товары,
        /// или подкаталоги
        /// </summary>
        /// <param name="catalogNode"></param>
        /// <returns></returns>
        public bool IsCatalogForMerches(HtmlNode catalogNode)
        {

            //data-meta-name="CategoryCardsLayout" - для каталога с каталогами
            //data-meta-name="ProductListLayout" - для каталога с товарами


            var possibleNodeInPageOfCatalogs = catalogNode.SelectSingleNode
                ("//*[@data-meta-name=\"CategoryCardsLayout\"]");
            var possibleNodeInPageOfMerches = catalogNode.SelectSingleNode
                ("//*[@data-meta-name=\"ProductListLayout\"]");

            if (possibleNodeInPageOfCatalogs != null && possibleNodeInPageOfMerches == null)
                return false;
            else if (possibleNodeInPageOfMerches != null && possibleNodeInPageOfCatalogs == null)
                return true;
            else
                throw new InvalidOperationException("Не удалось определить, содержит ли каталог товары," +
                    "или подкаталоги.");



        }


        /// <summary>
        /// Метод возвращает URL-адреса субкаталогов, содержащихся в другом субкаталоге.
        /// </summary>
        /// <param name="baseCatalogNode"></param>
        /// <returns>URL-адреса подкаталогов (содержащих либо товары, либо подкаталоги)</returns>
        public List<string> ParseSubCatalogsUrlsFromSub(HtmlNode baseCatalogNode)
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

            return urls;
        }

        public List<string> ParseSubCatalogsUrlsFromMain(HtmlNode mainCatalogNode)
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
            return hrefs;
        }



        public int ParsePageCount(HtmlNode catalogPage)
        {
            //   data-meta-name="Pagination"   для контейнера

            //   //div[contains(@data-meta-name, 'PaginationElement')][last()] для нахождения нужного узла внутри контейнера.

            var paginationNode = catalogPage.SelectSingleNode("//*[@data-meta-name=\"Pagination\"]");


            var lastPageNode = paginationNode?.SelectSingleNode("(.//div[contains(@data-meta-name, 'PaginationElement__page')])[last()]");


            var lastPageAsString = lastPageNode?.InnerText;

            if (lastPageAsString != null)
            {
                int pageCount = int.Parse(lastPageAsString);
                _logger?.LogDebug($"{nameof(ParsePageCount)}: получено число страниц - {pageCount}");
                return pageCount;
            }
            else
            {
                _logger?.LogWarning($"{nameof(ParsePageCount)}: либо страница одна - либо произошла непредвиденная ситуация.");
                return 1;
            }

        }


        public async Task<List<CitilinkMerchParsingDto>> ParsePortionFromUrl(string url)
        {
            var scrapTask = _scraper.ScrapProductPortionFromUrl(url);
            return ParsePortionFromHtml(await scrapTask);
        }
        protected List<CitilinkMerchParsingDto> ParsePortionFromHtml(HtmlNode htmlDocument)
        {
            var productListNode = htmlDocument.SelectSingleNode("//*[@data-meta-name=\"ProductListLayout\"]");

            if (productListNode == null)
                throw new InvalidOperationException("Не удалось найти узел со списком товаров.");
            //data-meta-product-id
            var nodes = productListNode.SelectNodes(".//*[@data-meta-product-id]");
            if (nodes == null)
                throw new InvalidOperationException("Не удалось спарсить citilink-товары: " +
                    "не удалось найти узлы с товарами.");
            List<CitilinkMerchParsingDto> merches = [];
            foreach (var node in nodes)
            {
                decimal price;
                try
                {
                    price = ParsePrice(node);
                }
                catch (InvalidOperationException)
                {
                    // TODO: Решает проблему того что товары которых нет в наличии не парсятся.
                    // Можно и переделать!
                    price = decimal.MinValue;
                    continue;
                }
                var citilinkId = ParseCitilinkId(node);
                var name = ParseName(node);
                merches.Add(new(price, citilinkId, name));
            }
            return merches;
        }

        protected decimal ParsePrice(HtmlNode baseNode)
        {
            var priceNode = baseNode.SelectSingleNode(".//*[@data-meta-price]");

            var attribute = priceNode?.GetDataAttribute("meta-price");

            var onFail = new InvalidOperationException("Не получилось извлечь цену.\n" +
                $"Тег извлечения(если есть): {priceNode?.OuterHtml}");
            decimal price;
            if (attribute != null)
            {
                try
                {
                    price = decimal.Parse(attribute.Value);
                }
                catch
                {
                    throw onFail;
                }
                return price;
            }
            else
                throw onFail;

        }
        protected string ParseCitilinkId(HtmlNode baseNode)
        {
            var attribute = baseNode.GetDataAttribute("meta-product-id");

            var onFail = new InvalidOperationException("Не получилось извлечь Id ситилинк-продукта.\n" +
                $"Тег для извлечения: {baseNode.OuterHtml}");
            string citilinkId;
            if (attribute != null)
            {
                try
                {
                    citilinkId = attribute.Value;
                }
                catch
                {
                    throw onFail;
                }
                return citilinkId;
            }
            else
                throw onFail;
        }

        protected string ParseName(HtmlNode baseNode)
        {
            /*
            var nameNode = baseNode.SelectSingleNode(".//*[@data-meta-name=\"Snippet__title\"]");
            string? name = nameNode?.InnerText;
            */

            var nameNode = baseNode.SelectSingleNode(".//img");
            string? name = nameNode?.GetAttributeValue("alt", "Not Found!!!");
            if (name == null || name == "Not Found!!!")
                throw new InvalidOperationException("Не удалось извлечь наименование citilink-товара.\n" +
                    $"Тег извлечения(если есть): {nameNode?.OuterHtml}");
            else
                return name;
        }

    }
}


