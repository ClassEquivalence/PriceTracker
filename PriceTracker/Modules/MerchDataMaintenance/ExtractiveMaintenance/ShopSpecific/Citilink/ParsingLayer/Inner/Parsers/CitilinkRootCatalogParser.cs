
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner.Parsers
{
    public class CitilinkRootCatalogParser : ICitilinkRootCatalogParser
    {

        private readonly CitilinkUpsertionOptions _options;
        private readonly ILogger? _logger;
        public CitilinkRootCatalogParser(IOptions<CitilinkUpsertionOptions> options,
            ILogger? logger = null)
        {
            _options = options.Value;
            _logger = logger;
            if (string.IsNullOrEmpty(_options.CitilinkUrl))
                throw new ArgumentException();
        }

        public async Task<List<string>> ParseUnitAsync(string parsable)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(parsable);
            var node = doc.DocumentNode;

            var hrefs = ParseSubCatalogsUrlsFromMain(node, true);
            foreach (var href in hrefs)
            {
                _logger?.LogTrace($"{href} as URL has been parsed");
            }
            return hrefs;
        }


        private List<string> ParseSubCatalogsUrlsFromMain(HtmlNode mainCatalogNode, bool shouldMakeUrlsAbsolute =
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

            if (shouldMakeUrlsAbsolute)
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

    }
}
