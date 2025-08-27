using HtmlAgilityPack;
using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper;

namespace PriceTracker.DisabledFiles
{
    public class MerchCatalogUrlsExtractor
    {
        const string merchCatalogPaginationQueryString = "?ref=mainmenu_plate&p=";
        const string baseUrl = "https://www.citilink.ru";

        private readonly ILogger? _logger;


        private readonly ICitilinkScraper _scraper;

        public MerchCatalogUrlsExtractor(ICitilinkScraper scraper, ILogger? logger = null)
        {
            // TODO: оптимизировать до нормальной асинхронности можно.
            _scraper = scraper;
            _logger = logger;
        }


        public async IAsyncEnumerable<string> RetrieveAllMerchCatalogsUrls()
        {
            _logger?.LogTrace($"{nameof(MerchCatalogUrlsExtractor)} {nameof(RetrieveAllMerchCatalogsUrls)}:" +
                $"Вытягивание информации о всех каталогах товаров начато.");

            var mainCatalogSectionsNode = await _scraper.UrlToNodeAsync("https://www.citilink.ru/catalog/");



            async IAsyncEnumerable<string> RecursiveMerchCatalogRetreive(string subCatalogRelativeUrl)
            {
                string subCatalogUrl = baseUrl + subCatalogRelativeUrl.
                    SubstringWithAndAfterFirstEntryOrEmpty("/catalog");
                _logger?.LogTrace($"{nameof(RecursiveMerchCatalogRetreive)}: " +
                    $"Сформирован URL: {subCatalogUrl}");

                var subCatalogNode = await _scraper.UrlToNodeAsync(subCatalogUrl);
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
            _logger?.LogTrace($"{nameof(RetrieveAllMerchCatalogsUrls)}:" +
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





    }
}
