using HtmlAgilityPack;
using Microsoft.Playwright;
using PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine.ScrapingServices.HttpClients.Browser;
using System;

namespace PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine.ShopSpecific.Citilink
{
    public class CitilinkScraper
    {
        private readonly BrowserAdapter _browser;
        private readonly ILogger? _logger;
        private readonly int _functionWaitPollingInterval;
        private readonly int _maxAddressBarRequests;
        private int requestIteration = 0;

        public CitilinkScraper(BrowserAdapter browserAdapter, ILogger? logger = null,
            int maxAddressBarGetRequestsCount = 300)
        {
            _browser = browserAdapter;
            _logger = logger;
            _functionWaitPollingInterval = 1000;
            _maxAddressBarRequests = maxAddressBarGetRequestsCount;
        }

        public async Task<HtmlNode> UrlToNode(string url)
        {
            _logger?.LogDebug($"{nameof(UrlToNode)}: превращаем {url} в узел.");
            string html = await _browser.UrlToHtmlAsync(url);
            return HtmlToNode(html);
        }

        public HtmlNode HtmlToNode(string html)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(html);
            return doc.DocumentNode;
        }

        public async Task<HtmlNode> ScrapProductPortionFromUrl(string url)
        {
            _logger?.LogDebug($"{nameof(ScrapProductPortionFromUrl)}: попытка взять порцию из {url}.");
            int attemptCounts = 3;
            string html;
            for (int i = 1; i <= attemptCounts; i++)
            {
                try
                {
                    await GotoAsync(url);

                    await WaitForPortionLoadedAsync();

                    break;
                }
                catch (TimeoutException ex)
                {
                    _logger?.LogTrace($"{ScrapProductPortionFromUrl}: Не вышло извлечь данные с {i}-го раза.");

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

        private async Task<string> GetHtmlContentAsync()
        {
            string html;
            html = await _browser.GetHtmlContentAsync();
            return html;
        }

        private async Task GotoAsync(string url)
        {
            if (requestIteration < _maxAddressBarRequests)
                requestIteration++;
            else
                throw new InvalidOperationException($"{GotoAsync}: Исчерпан лимит запросов " +
                    $"{_maxAddressBarRequests}");
            await _browser.GotoAsync(url);
        }
        private async Task WaitForPortionLoadedAsync()
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
    }
}
