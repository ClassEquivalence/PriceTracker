using Microsoft.Playwright;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Utils.ScrapingServices.HttpClients.Browser
{
    public class BrowserAdapter
    {
        private readonly IBrowser _browser;
        private IBrowserContext _browserContext;
        private IPage _page;
        // Delay between requests
        private readonly (float minDelay, float maxDelay) _delayRange;
        private readonly ILogger? _logger;
        private readonly Random _randomGenerator;
        public BrowserAdapter(IBrowser browser, (float minDelay, float maxDelay) delayRange,
            ILogger? logger = null, Random? numberGenerator = null)
        {
            _browser = browser;
            _browserContext = _browser.NewContextAsync().Result;


            _page = _browserContext.NewPageAsync().Result;
            _delayRange = delayRange;
            _logger = logger;

            _randomGenerator = numberGenerator ?? new Random();
        }

        private int generateDelay()
        {
            float delayInSeconds = _randomGenerator.NextSingle() * (_delayRange.maxDelay - _delayRange.minDelay)
                + _delayRange.minDelay;

            return Convert.ToInt32(delayInSeconds * 1000);
        }
        public async Task GotoAsync(string url, int attemptCount = 4)
        {
            int i = 1;
            for (i = 1; i <= attemptCount; i++)
            {
                try
                {
                    _logger?.LogTrace($"{nameof(GotoAsync)}: переход к странице {url}");
                    var delay = Task.Delay(generateDelay());
                    var response = await _page.GotoAsync(url, new() { WaitUntil = WaitUntilState.DOMContentLoaded });
                    
                    await delay;
                    break;
                }
                catch (TimeoutException ex)
                {
                    _logger?.LogTrace($"Не вышло загрузить страницу {url} с {i}-го раза");
                    if (i == attemptCount)
                        throw new TimeoutException($"Не вышло загрузить страницу {url} с {i}-го раза",
                            ex);
                }
            }

        }

        public async Task<string> GetHtmlContentAsync(int maxAttempts = 20, int msDelayBetweenAttempts = 2000)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxAttempts, nameof(maxAttempts));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(msDelayBetweenAttempts, nameof(maxAttempts));

            string html;
            int i = 1;
            try
            {
                html = await _page.ContentAsync();
                for (i = 1; i < maxAttempts; i++)
                    html = await _page.ContentAsync();
                return html;
            }
            catch (PlaywrightException e)
            {
                _logger?.LogWarning(e.Message, e);
                await Task.Delay(msDelayBetweenAttempts);
                if (i == maxAttempts - 1)
                {
                    throw new PlaywrightException($"Не удалось извлечь данные страницы с {maxAttempts} попыток.", e);
                }
            }
            html = await _page.ContentAsync();
            return html;
        }

        public async Task<string> UrlToHtmlAsync(string url, int maxAttempts = 20,
            int msDelayBetweenAttempts = 2000)
        {
            await GotoAsync(url);
            return await GetHtmlContentAsync(maxAttempts, msDelayBetweenAttempts);
        }

        public async Task WaitForFunctionAsync(string func, int pollingInterval = 1000)
        {
            await _page.WaitForFunctionAsync(func, new PageWaitForFunctionOptions()
            { PollingInterval = pollingInterval });
        }

        public async Task<string> GetStorageStateAsync()
        {
            return await _browserContext.StorageStateAsync();
        }

        public async Task LoadStorageStateAsync(string storageState)
        {

            var previousContext = _browserContext;

            var taskNewContext = _browser.NewContextAsync(new() { StorageState = storageState });
            var taskDisposePreviousContext = previousContext.DisposeAsync();

            _browserContext = await taskNewContext;
            var taskNewPage = _browserContext.NewPageAsync();
            _page = await taskNewPage;
            await taskDisposePreviousContext;
        }

        public async Task ReloadAsync()
        {
            await _page.ReloadAsync();
        }
    }
}
