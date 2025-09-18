using HtmlAgilityPack;
using PriceTracker.Core.Utils;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper
{
    public interface ICitilinkScraper
    {
        public Task<FunctionResult<HtmlNode?, HtmlNodeRequestInfo>> UrlToNodeAsync(string url, 
            int retryIntervalSeconds = 30, int maxAttemptCount = 5);


        public enum HtmlNodeRequestInfo
        {
            SeeminglyOk,
            TooManyRequests,
            NotFound,
            Error
        }

        /// <summary>
        /// page начинается с единицы.
        /// categorySlug - название категории(каталога товаров) на латинице.
        /// <br/>
        /// Не забудь dispose response.
        /// <br/>
        /// Метод возвращает ответ.
        /// </summary>
        /// <param name="categorySlug"></param>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ScrapProductPortionAsJsonAsync(string categorySlug, int page, int perPage = 1000,
            string? cookie = default, int retryIntervalSeconds = 30, int maxAttemptCount = 5);


    }
}
