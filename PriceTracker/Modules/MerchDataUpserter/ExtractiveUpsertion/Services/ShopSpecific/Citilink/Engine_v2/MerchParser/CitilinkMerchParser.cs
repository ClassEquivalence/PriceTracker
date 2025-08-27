using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.MerchParser.DeserializedMerchFetchResponse;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.MerchParser
{
    public class CitilinkMerchParser : ICitilinkMerchParser
    {



        private readonly ILogger? _logger;

        private List<string> ignoredCategorySlugs;


        private readonly ICitilinkScraper _scraper;

        public CitilinkMerchParser(ICitilinkScraper scraper, ILogger? logger = null, 
            List<string>? ignoredCategorySlugs = null)
        {
            // TODO: оптимизировать до нормальной асинхронности можно.
            _scraper = scraper;
            _logger = logger;
            this.ignoredCategorySlugs = ignoredCategorySlugs ?? [];
        }


        public enum RetreiveAllFromMerchCatalog_ExecState
        {
            Success,
            ServerGrownTired,
            PassedIgnoredCategorySlug,
            UnknownServerError
        }

        //IAsyncEnumerable<CitilinkMerchParsingDto>
        public async Task<FunctionResult<IAsyncEnumerable<CitilinkMerchParsingDto>,
            RetreiveAllFromMerchCatalog_ExecState>> RetreiveAllFromMerchCatalog(BranchWithHtml catalog)
        {
            _logger?.LogTrace($"{nameof(CitilinkMerchParser)}, {RetreiveAllFromMerchCatalog}: \n" +
                $"Метод был вызван.");

            if (ignoredCategorySlugs.Contains(catalog.GetCategorySlug()))
            {
                return new FunctionResult<IAsyncEnumerable<CitilinkMerchParsingDto>, RetreiveAllFromMerchCatalog_ExecState>
                    (EmptyAsync.GetEmptyAsyncEnumerable<CitilinkMerchParsingDto>(), 
                    RetreiveAllFromMerchCatalog_ExecState.PassedIgnoredCategorySlug);

            }

            var fetchRequestResult = await SendRequest(catalog.GetCategorySlug(), 1);

            switch (fetchRequestResult.Info)
            {
                case SendRequest_ExecInfo.ServerTired:
                    return new(EmptyAsync.GetEmptyAsyncEnumerable<CitilinkMerchParsingDto>(),
                    RetreiveAllFromMerchCatalog_ExecState.ServerGrownTired);

                case SendRequest_ExecInfo.UnknownServerError:
                    return new(EmptyAsync.GetEmptyAsyncEnumerable<CitilinkMerchParsingDto>(),
                    RetreiveAllFromMerchCatalog_ExecState.UnknownServerError);
            }

            var responseDeserialized = fetchRequestResult.Result;

            int pageCount = GetPageCount(responseDeserialized);
            var dtos = ExtractDtos(responseDeserialized);

            SendRequest_ExecInfo request_ExecInfo = SendRequest_ExecInfo.Success;

            var merchDtos = Iterator();

            RetreiveAllFromMerchCatalog_ExecState retreiveInfo;

            switch (request_ExecInfo)
            {
                case SendRequest_ExecInfo.ServerTired:
                    retreiveInfo = RetreiveAllFromMerchCatalog_ExecState.ServerGrownTired;
                    break;
                case SendRequest_ExecInfo.UnknownServerError:
                    retreiveInfo = RetreiveAllFromMerchCatalog_ExecState.UnknownServerError;
                    break;
                default:
                    retreiveInfo = RetreiveAllFromMerchCatalog_ExecState.Success;
                    break;
            }

            return new(merchDtos, retreiveInfo);

            async IAsyncEnumerable<CitilinkMerchParsingDto> Iterator()
            {
                foreach (var dto in dtos)
                {
                    yield return dto;
                }

                for (int i = 2; i <= pageCount; i++)
                {
                    fetchRequestResult = await SendRequest(catalog.GetCategorySlug(), i);
                    responseDeserialized = fetchRequestResult.Result;

                    request_ExecInfo = fetchRequestResult.Info;
                    if (request_ExecInfo != SendRequest_ExecInfo.Success)
                        yield break;

                    dtos = ExtractDtos(responseDeserialized);

                    foreach (var dto in dtos)
                    {
                        yield return dto;
                    }
                }
            }

        }

        private static int GetPageCount(Response response)
        {
            return response.data.productsFilter.record.pageInfo.totalPages;
        }

        private static List<CitilinkMerchParsingDto> ExtractDtos(Response response)
        {
            var products = response.data.productsFilter.record.products;

            List<CitilinkMerchParsingDto> merchParsingDtos = [];
            foreach(var product in products)
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

        private async Task<Response> StreamToDeserializedResponseAsync(Stream responseStream)
        {

            using StreamReader responseStreamReader = new(responseStream, Encoding.UTF8);

            var responseString = await responseStreamReader.ReadToEndAsync();

            await Task.Delay(10000);

            var responseDeserialized = JsonSerializer.Deserialize<Response>(responseString);
            return responseDeserialized;
        }



        public enum SendRequest_ExecInfo
        {
            Success,
            ServerTired,
            UnknownServerError
        }

        /// <summary>
        /// Отправить fetch-запрос к ситилинку на получение товаров.
        /// На выходе получить json-десериализованный объект с товарами и доп. данными.
        /// </summary>
        /// <returns></returns>
        private async Task<FunctionResult<Response, SendRequest_ExecInfo>> 
            SendRequest(string categorySlug, int page, int perPage = 1000,
            string? cookie = default)
        {
            _logger?.LogTrace($"{nameof(CitilinkMerchParser)}, {nameof(SendRequest)}: \n" +
                $"Информация Fetch-запроса: categorySlug = {categorySlug},\n " +
                $"page = {page}, perPage = {perPage}");

            var response = await _scraper.ScrapProductPortionAsJsonAsync(categorySlug, page, 
                perPage, cookie);
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                return new(null, SendRequest_ExecInfo.ServerTired);
            }
            else if(response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new(null, SendRequest_ExecInfo.UnknownServerError);
            }
            
            var responseStream = await response.Content.ReadAsStreamAsync();
            var responseDeserialized = await StreamToDeserializedResponseAsync(responseStream);
            if (responseDeserialized == null)
                throw new InvalidOperationException($"{nameof(CitilinkMerchParser)}, {nameof(SendRequest)}:" +
                    $" ");
            await responseStream.DisposeAsync();
            return new(responseDeserialized, SendRequest_ExecInfo.Success);
        }

    }
}
