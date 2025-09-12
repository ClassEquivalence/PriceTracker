using HtmlAgilityPack;
using PriceTracker.Core.Configuration.ProvidedWithDI;
using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Utils.ScrapingServices.HttpClients.Browser;
using System.Text.Json;
using static PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper.ICitilinkScraper;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.Scraper
{
    public class CitilinkScraper: ICitilinkScraper
    {
        private readonly HttpClient _baseClient;

        private readonly MerchFetchRequestBuilder _merchFetchRequestBuilder;


        private readonly BrowserAdapter _browser;
        private readonly ILogger? _logger;
        private readonly string _citilinkCatalogPageUrl = "https://www.citilink.ru/catalog/";
        private readonly CitilinkUpsertionOptions _options;

        private int requestCount;

        private readonly int _maxRequestsPerTime;

        public event Action? RequestLimitReached;

        public CitilinkScraper(BrowserAdapter browserAdapter, int maxRequestsPerTime,
            CitilinkUpsertionOptions options, ILogger? logger = null)
        {
            _merchFetchRequestBuilder = new(options.CitilinkAPIRoute);

            _baseClient = new HttpClient();
            //_baseClient.DefaultRequestHeaders.UserAgent


            _browser = browserAdapter;
            _logger = logger;
            requestCount = 0;
            _maxRequestsPerTime = maxRequestsPerTime;
            _options = options;

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
            string html = await _browser.UrlToHtmlAsync(url);
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


        /*
        public async Task<HtmlNode> ScrapProductPortionAsHtmlAsync(string url, int attemptCounts = 10)
        {

            _logger?.LogDebug($"{nameof(ScrapProductPortionAsHtmlAsync)}: попытка взять порцию из {url}.");
            string html;
            for (int i = 1; i <= attemptCounts; i++)
            {
                try
                {
                    await GotoAsync(url);

                    await WaitForProductPortionLoadedAsync();

                    break;
                }
                catch (TimeoutException ex)
                {
                    _logger?.LogTrace($"{ScrapProductPortionAsHtmlAsync}: Не вышло извлечь данные с {i}-го раза.");

                    //html = await GetHtmlContentAsync();
                    //_logger?.LogTrace($"{ScrapProductPortionFromUrl}: извлеченный html:\n {html}");

                    if (i == attemptCounts)
                    {
                        throw new TimeoutException($"Не вышло извлечь данные с {i}-х раз.", ex);
                    }
                }
            }
            html = await GetHtmlContentAsync();
            //_logger?.LogTrace($"{ScrapProductPortionFromUrl}: извлеченный html:\n {html}");
            return HtmlToNode(html);
        }
        */


        private async Task<string> GetHtmlContentAsync()
        {
            string html;
            html = await _browser.GetHtmlContentAsync();
            return html;
        }

        private async Task GotoAsync(string url)
        {
            url = url.TrimEnd('/');
            await _browser.GotoAsync(url);
        }
        private async Task WaitForProductPortionLoadedAsync()
        {
            await _browser.WaitForFunctionAsync(
                @"() => {
                //if (document.readyState !== 'complete') return false;

                const items = document.querySelectorAll('[data-meta-product-id]');
                if (items.length === 0) return false;

                return Array.from(items).every(el =>
                    el.getAttribute('data-meta-product-id')?.trim().length > 0
                    );
                }");
        }

        public void RefreshRequestsCount()
        {
            requestCount = 0;
        }

        public async Task PerformInitialRunupAsync(string? storageState = null)
        {
            _logger?.LogDebug("Начальная инициализация скрапера ситилинка началась.");
            if (!string.IsNullOrWhiteSpace(storageState))
            {
                await _browser.LoadStorageStateAsync(storageState);
                _logger?.LogDebug("Загружен storagestate браузера для ситилинка." +
                    "\nИнициализация скрапера ситилинка завершилась.");
            }
            else
            {
                var gotoTask = _browser.GotoAsync(_citilinkCatalogPageUrl);
                var minimumWaitTask = Task.Delay(10000);
                await Task.WhenAll([gotoTask, minimumWaitTask]);
                await _browser.ReloadAsync();
                requestCount += 2;
                if (requestCount >= _maxRequestsPerTime)
                {
                    RequestLimitReached?.Invoke();
                }
                _logger?.LogDebug("Созданы новые файлы куки для скрапера ситилинка." +
                    "\nИнициализация скрапера ситилинка завершилась.");
            }
        }
        public async Task<string> GetStorageStateAsync()
        {
            return await _browser.GetStorageStateAsync();
        }

    }
}
