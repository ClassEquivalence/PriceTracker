using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner.Parsers
{
    public class CitilinkCatalogsCatalogParser : ICitilinkCatalogsCatalogParser
    {
        private readonly CitilinkUpsertionOptions _options;
        public CitilinkCatalogsCatalogParser(IOptions<CitilinkUpsertionOptions> options)
        {
            _options = options.Value;
        }

        public async Task<List<string>> ParseUnitAsync(string parsable)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(parsable);
            var node = doc.DocumentNode;
            return ParseSubCatalogsUrlsFromSub(node, true);
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

    }
}
