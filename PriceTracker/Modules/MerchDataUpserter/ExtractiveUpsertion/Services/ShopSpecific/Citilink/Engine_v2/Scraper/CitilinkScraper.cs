using HtmlAgilityPack;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Core.Utils;
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
            string? cookie = default)
        {
            var request = _merchFetchRequestBuilder.Build(categorySlug, page, perPage, cookie);

            var response = await _baseClient.SendAsync(request);
            requestCount++;
            if (requestCount >= _maxRequestsPerTime)
            {
                RequestLimitReached?.Invoke();
            }
            return response;
        }


        public async Task<FunctionResult<HtmlNode, HtmlNodeRequestInfo>> UrlToNodeAsync(string url)
        {
            _logger?.LogDebug($"{nameof(UrlToNodeAsync)}: превращаем {url} в узел.");
            string html = await _baseClient.GetStringAsync(url);
            requestCount++;
            if (requestCount >= _maxRequestsPerTime)
            {
                RequestLimitReached?.Invoke();
            }
            return HtmlToNode(html);
        }

        public FunctionResult<HtmlNode, HtmlNodeRequestInfo> HtmlToNode(string html)
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


    }
}
