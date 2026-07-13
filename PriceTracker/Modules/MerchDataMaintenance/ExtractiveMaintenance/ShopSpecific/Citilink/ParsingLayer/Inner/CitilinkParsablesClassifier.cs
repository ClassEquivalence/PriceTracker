using HtmlAgilityPack;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.DeserializedMerchFetchResponse;
using System.Text.Json;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner
{
    public class CitilinkParsablesClassifier : ICitilinkParsableClassifier
    {
        public CitilinkParsablesClassifier()
        {
        }

        public ParsableType Classify(string parsable)
        {
            if (IsJson(parsable))
            {
                if (IsJsonOfMerches(parsable))
                    return ParsableType.MerchesJson;
                else
                    return ParsableType.Unknown;
            }
            else if (IsHtml(parsable))
            {
                return ParseHtmlFunctionality(parsable) switch
                {
                    HtmlFunctionality.MerchCatalog => ParsableType.MerchCatalog,
                    HtmlFunctionality.RootCatalog => ParsableType.RootCatalog,
                    HtmlFunctionality.CatalogsCatalog => ParsableType.CatalogsCatalog,
                    _ => ParsableType.Unknown
                };
            }
            else
            {
                return ParsableType.Unknown;
            }
        }

        private static bool IsHtml(string parsable)
        {
            // Точка для оптимизации: возможно пересоздание-перепарсинг одного и того же html.
            if (string.IsNullOrEmpty(parsable))
                return false;

            HtmlDocument doc = new();
            doc.LoadHtml(parsable);
            if (doc.ParseErrors != null && doc.ParseErrors.Any())
                return false;
            else
                return true;
        }

        private static bool IsJson(string parsable)
        {
            // Точка для оптимизации: исключение
            try
            {
                JsonDocument.Parse(parsable);
            }
            catch (JsonException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// ОБЯЗАТЕЛЬНО ЗАКИНУТЬ В ЮНИТ-ТЕСТИРОВАНИЕ
        /// </summary>
        /// <param name="parsable"></param>
        /// <returns></returns>
        private static bool IsJsonOfMerches(string parsable)
        {
            Response? responseDeserialized;
            try
            {
                responseDeserialized = JsonSerializer.Deserialize<Response>(parsable);
            }
            catch (JsonException)
            {
                return false;
            }
            return true;
        }



        enum HtmlFunctionality
        {
            MerchCatalog, RootCatalog, CatalogsCatalog, Unknown
        }
        /// <summary>
        /// ОБЯЗАТЕЛЬНО ЗАКИНУТЬ В ИНТЕГРАЦИОННОЕ ТЕСТИРОВАНИЕ
        /// </summary>
        /// <param name="parsable"></param>
        /// <returns></returns>
        private static HtmlFunctionality ParseHtmlFunctionality(string parsable)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(parsable);
            var node = doc.DocumentNode;

            var possibleNodeInPageOfCatalogs = node.SelectSingleNode
                ("//*[@data-meta-name=\"CategoryCardsLayout\"]");
            var possibleNodeInPageOfMerches = node.SelectSingleNode
                ("//*[@data-meta-name=\"ProductListLayout\"]");
            var possibleNodeInRootCatalogPage = node.SelectSingleNode
                ("//link[@rel=\"canonical\"][@href=\"https://www.citilink.ru/catalog\"]");

            bool catalogsCatalogSignature = possibleNodeInPageOfCatalogs is not null;
            bool merchesCatalogSignature = possibleNodeInPageOfMerches is not null;
            bool rootSignature = possibleNodeInRootCatalogPage is not null;

            var signatures = (catalogsCatalogSignature, merchesCatalogSignature, rootSignature);

            return signatures switch
            {
                (true, false, false) => HtmlFunctionality.CatalogsCatalog,
                (false, true, false) => HtmlFunctionality.MerchCatalog,
                (false, false, true) => HtmlFunctionality.RootCatalog,
                _ => HtmlFunctionality.Unknown
            };

        }
    }
}

