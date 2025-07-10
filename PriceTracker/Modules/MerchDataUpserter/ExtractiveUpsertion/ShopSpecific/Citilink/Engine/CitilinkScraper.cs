using HtmlAgilityPack;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Utils.ScrapingServices.HttpClients.Browser;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink.Engine
{
    public class CitilinkScraper
    {
        private readonly BrowserAdapter _browser;
        private readonly ILogger? _logger;


        private readonly (int documentRequestCount, TimeSpan period) _maxPageRequestPerTime;
        private int requestIteration = 0;

        private DateTime lastTimeRequestHappened;

        public DateTime WhenRequestingAvailable
        {
            get
            {
                return (requestIteration < _maxPageRequestPerTime.documentRequestCount) ?
                    DateTime.Now : lastTimeRequestHappened;
            }
        }
        public event Action<DateTime>? MaxPageRequestPerTimeReached;

        public CitilinkScraper(BrowserAdapter browserAdapter, ILogger? logger = null,
            (int documentRequestCount, TimeSpan period)? maxPageRequestPerTime = null)
        {
            lastTimeRequestHappened = DateTime.Now;

            _maxPageRequestPerTime = maxPageRequestPerTime ??
                (300, TimeSpan.FromHours(12));

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
            if (TryPassRequestPerTimeLimit())
                await _browser.GotoAsync(url);
            else
                throw new InvalidOperationException($"{nameof(CitilinkScraper)}" +
                    $" недоступен в силу достижения указанного PageRequestsPerTime" +
                    $" лимита. Вызвать метод следовало бы позже.");
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


        // TODO: Метод несовершенен, и может запрещать доступ тогда, когда
        // он должен быть разрешен. Но не наоборот.
        private bool TryPassRequestPerTimeLimit()
        {
            if (DateTime.Now > lastTimeRequestHappened)
            {
                requestIteration = 1;

                lastTimeRequestHappened = DateTime.Now + _maxPageRequestPerTime.period;
                return true;
            }
            else if (requestIteration < _maxPageRequestPerTime.documentRequestCount)
            {
                requestIteration++;

                lastTimeRequestHappened = DateTime.Now + _maxPageRequestPerTime.period;

                if (requestIteration == _maxPageRequestPerTime.documentRequestCount - 1)
                {
                    MaxPageRequestPerTimeReached?.Invoke(lastTimeRequestHappened);
                }

                return true;
            }
            else
            {
                MaxPageRequestPerTimeReached?.Invoke(lastTimeRequestHappened);
                return false;
            }


            // если возвращаем true - то WhenRequestingAvailable = DateTime.Now + period;
            // если false то этого не нужно.
        }
    }
}
