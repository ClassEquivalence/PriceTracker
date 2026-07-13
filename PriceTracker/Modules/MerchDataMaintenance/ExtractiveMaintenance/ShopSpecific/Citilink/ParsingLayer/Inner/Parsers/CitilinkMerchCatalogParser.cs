
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.DeserializedMerchFetchResponse;
using System.Text.Json;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner.Parsers
{
    public class CitilinkMerchCatalogParser : ICitilinkMerchCatalogParser
    {
        private readonly ILogger? _logger;
        public CitilinkMerchCatalogParser(ILogger? logger = null)
        {
            _logger = logger;
        }

        public async Task<List<CitilinkMerchParsingDto>> ParseMerchesAsync(string parsableJson)
        {
            var response = DeserializeResponse(parsableJson);
            var parsedMerches = ExtractDtos(response, _logger);
            return parsedMerches;
        }

        public async Task<int> ParseMerchCountAsync(string parsableJson)
        {
            var response = DeserializeResponse(parsableJson);
            return response.data.productsFilter.record.pageInfo.totalItems;
        }

        private static List<CitilinkMerchParsingDto> ExtractDtos(Response response,
            ILogger? logger = null)
        {
            logger?.LogTrace($"{nameof(ParseMerchesAsync)}: response is {response}");
            logger?.LogTrace($"{nameof(ParseMerchesAsync)}: data is {response.data}");
            logger?.LogTrace($"{nameof(ParseMerchesAsync)}: productsFilter is " +
                $"{response.data.productsFilter}");
            logger?.LogTrace($"{nameof(ParseMerchesAsync)}: record is " +
                $"{response.data.productsFilter.record}");
            logger?.LogTrace($"{nameof(ParseMerchesAsync)}: products is " +
                $"{response.data.productsFilter.record.products}");

            var products = response.data.productsFilter.record.products;
            

            List<CitilinkMerchParsingDto> merchParsingDtos = [];

            logger?.LogTrace($"Products count is: {products.Count()}");

            foreach (var product in products)
            {
                if (string.IsNullOrEmpty(product.price.current))
                    continue;

                decimal.TryParse(product.price.current, out var price);
                CitilinkMerchParsingDto merch = new(price,
                    product.id, product.name);
                merchParsingDtos.Add(merch);
            }

            return merchParsingDtos;
        }


        private Response DeserializeResponse(string parsable)
        {
            var responseDeserialized = JsonSerializer.Deserialize<Response>(parsable);
            return responseDeserialized;
        }


    }
}
