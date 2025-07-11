using HtmlAgilityPack;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Utils.ScrapingServices.HttpClients.Browser;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink.Engine
{
    public class CitilinkScraper: ICitilinkScraper
    {
        private readonly BrowserAdapter _browser;
        private readonly ILogger? _logger;
        private readonly string _citilinkCatalogPageUrl = "https://www.citilink.ru/catalog/";


        public CitilinkScraper(BrowserAdapter browserAdapter, ILogger? logger = null)
        {

            _browser = browserAdapter;
            _logger = logger;
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

                    await WaitForProductPortionLoadedAsync();

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

        public async Task PerformInitialRunup(string? storageState = null)
        {
            if(storageState!=null)
                await _browser.LoadStorageStateAsync(storageState);
            else
            {
                var gotoTask = _browser.GotoAsync(_citilinkCatalogPageUrl);
                var minimumWaitTask = Task.Delay(10000);
                await Task.WhenAll([gotoTask, minimumWaitTask]);
            }
        }
        public async Task<string> GetStorageStateAsync()
        {
            return await _browser.GetStorageStateAsync();
        }
    }
}
