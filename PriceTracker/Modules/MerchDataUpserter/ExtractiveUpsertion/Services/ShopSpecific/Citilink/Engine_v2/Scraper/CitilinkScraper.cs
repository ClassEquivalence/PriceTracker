using HtmlAgilityPack;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Core.Utils;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using static PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper.ICitilinkScraper;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper
{
    public class CitilinkScraper: ICitilinkScraper
    {
        private readonly HttpClient _baseClient;

        private readonly MerchFetchRequestBuilder _merchFetchRequestBuilder;

        private readonly ILogger? _logger;
        private readonly string _citilinkCatalogPageUrl;
        private readonly CitilinkUpsertionOptions _options;

        private int requestCount;

        private readonly int _maxRequestsPerTime;

        public event Action? RequestLimitReached;

        public CitilinkScraper(CitilinkUpsertionOptions options, string userAgent, ILogger? logger = null)
        {
            _merchFetchRequestBuilder = new(options.CitilinkAPIRoute);


            _baseClient = new HttpClient();
            _baseClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
            _baseClient.DefaultRequestHeaders.Add("Cookie", options.CitilinkHttpClientCookie);


            _logger = logger;
            requestCount = 0;
            _maxRequestsPerTime = options.MaxPageRequestsPerTime;
            _options = options;
            _citilinkCatalogPageUrl = _options.CitilinkMainCatalogUrl;
        }


        public async Task<HttpResponseMessage> ScrapProductPortionAsJsonAsync(string categorySlug, int page, int perPage = 1000,
            string? cookie = default, int retryIntervalSeconds = 30, int maxAttemptCount = 5)
        {

            ArgumentOutOfRangeException.ThrowIfLessThan(retryIntervalSeconds, 0,
                $"{nameof(CitilinkScraper)}, {nameof(ScrapProductPortionAsJsonAsync)}: " +
                $"время для повторения запроса не может быть отрицательным.");
            ArgumentOutOfRangeException.ThrowIfLessThan(maxAttemptCount, 1,
                $"{nameof(CitilinkScraper)}, {nameof(ScrapProductPortionAsJsonAsync)}: " +
                $"число попыток запроса не должно быть меньше 1.");

            using var request = _merchFetchRequestBuilder.Build(categorySlug, page, perPage, cookie);

            HttpResponseMessage? response = null;

            int attempt = 1;
            do
            {

                

                if (response != null)
                    response.Dispose();

                response = await _baseClient.SendAsync(request);
                requestCount++;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    break;
                }
                else
                {
                    _logger?.LogTrace($"{nameof(CitilinkScraper)}, {nameof(ScrapProductPortionAsJsonAsync)}: " +
                    $"Попытка N {attempt} взять список товаров провалилась ({response.StatusCode})");
                }
                await Task.Delay(TimeSpan.FromSeconds(retryIntervalSeconds));
                attempt++;
            } while (attempt <= maxAttemptCount);

            
            if (requestCount >= _maxRequestsPerTime)
            {
                RequestLimitReached?.Invoke();
            }
            return response;
        }


        public async Task<FunctionResult<HtmlNode?, HtmlNodeRequestInfo>> UrlToNodeAsync(string url,
            int retryIntervalSeconds = 30, int maxAttemptCount = 5)
        {

            ArgumentOutOfRangeException.ThrowIfLessThan(retryIntervalSeconds, 0,
                $"{nameof(CitilinkScraper)}, {nameof(UrlToNodeAsync)}: время для повторения запроса не может" +
                $" быть отрицательным.");
            ArgumentOutOfRangeException.ThrowIfLessThan(maxAttemptCount, 1,
                $"{nameof(CitilinkScraper)}, {nameof(UrlToNodeAsync)}: число попыток запроса не должно быть" +
                $" меньше 1.");

            _logger?.LogTrace($"{nameof(CitilinkScraper)}, {nameof(UrlToNodeAsync)}: превращаем {url} в узел.");


            HttpResponseMessage? response = null;

            for(int attempt = 1; attempt <= maxAttemptCount; attempt++)
            {
                response?.Dispose();

                response = await _baseClient.GetAsync(url);

                requestCount++;
                if (requestCount >= _maxRequestsPerTime)
                {
                    RequestLimitReached?.Invoke();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.OK
                    || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    break;
                await Task.Delay(TimeSpan.FromSeconds(retryIntervalSeconds));
            }

            if (response is null)
                return new(null, HtmlNodeRequestInfo.Error);

            using (response)
            {
                return response.StatusCode switch
                {
                    HttpStatusCode.OK => HtmlToNode(await response.Content.ReadAsStringAsync()),
                    HttpStatusCode.NotFound => new(null, HtmlNodeRequestInfo.NotFound),
                    HttpStatusCode.TooManyRequests => new(null, HtmlNodeRequestInfo.TooManyRequests),
                    _ => new(null, HtmlNodeRequestInfo.Error)
                };
            }

            
        }

        private FunctionResult<HtmlNode?, HtmlNodeRequestInfo> HtmlToNode(string html)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(html);
            var root = doc.DocumentNode;
            
            var statusNode = root.SelectSingleNode("//div[@class=\"container__status\"]");
            if (statusNode!=null && statusNode.InnerText.Contains("429"))
            {
                return new(root, HtmlNodeRequestInfo.TooManyRequests);
            }

            return new(root, HtmlNodeRequestInfo.SeeminglyOk);
        }

        public void RefreshRequestsCount()
        {
            requestCount = 0;
        }

        ~CitilinkScraper()
        {
            _baseClient.Dispose();
        }

    }
}
