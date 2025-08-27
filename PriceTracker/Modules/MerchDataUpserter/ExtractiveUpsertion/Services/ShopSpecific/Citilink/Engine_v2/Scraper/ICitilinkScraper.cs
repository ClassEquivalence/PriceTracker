using HtmlAgilityPack;
using PriceTracker.Core.Utils;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper
{
    public interface ICitilinkScraper
    {
        public Task<FunctionResult<HtmlNode, HtmlNodeRequestInfo>> UrlToNodeAsync(string url);


        public enum HtmlNodeRequestInfo
        {
            SeeminglyOk,
            TooManyRequests
        }
        public FunctionResult<HtmlNode, HtmlNodeRequestInfo> HtmlToNode(string html);

        //public Task<HtmlNode> ScrapProductPortionAsHtmlAsync(string url, int attemptCounts = 10);

        /// <summary>
        /// page начинается с единицы.
        /// categorySlug - название категории(каталога товаров) на латинице.
        /// <br/>
        /// Не забудь dispose stream.
        /// <br/>
        /// Метод возвращает ответ.
        /// </summary>
        /// <param name="categorySlug"></param>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ScrapProductPortionAsJsonAsync(string categorySlug, int page, int perPage = 1000,
            string? cookie = default);

        public Task PerformInitialRunupAsync(string? storageState = null);
        public Task<string> GetStorageStateAsync();


    }
}
