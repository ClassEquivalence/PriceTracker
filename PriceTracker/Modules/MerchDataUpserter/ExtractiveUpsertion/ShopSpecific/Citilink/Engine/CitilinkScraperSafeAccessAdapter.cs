using HtmlAgilityPack;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.ShopSpecific.Citilink.Engine
{
    public class CitilinkScraperSafeAccessAdapter : ICitilinkScraper
    {

        private readonly (int documentRequestCount, TimeSpan period) _maxPageRequestPerTime;
        private int requestIteration = 0;

        private DateTime lastTimeRequestHappened;


        private readonly ICitilinkScraper _baseScraper;
        public CitilinkScraperSafeAccessAdapter(ICitilinkScraper baseScraper,
            (int documentRequestCount, TimeSpan period)? maxPageRequestPerTime)
        {

            lastTimeRequestHappened = DateTime.Now;

            _maxPageRequestPerTime = maxPageRequestPerTime ??
                (300, TimeSpan.FromHours(12));

            _baseScraper = baseScraper;
        }

        public HtmlNode HtmlToNode(string html)
        {
            
            return _baseScraper.HtmlToNode(html);
        }

        public async Task<HtmlNode> ScrapProductPortionFromUrl(string url)
        {
            if (!TryPassRequestPerTimeLimit())
            {
                await Task.Delay(lastTimeRequestHappened + _maxPageRequestPerTime.period
                    - DateTime.Now);
            }
            return await _baseScraper.ScrapProductPortionFromUrl(url);
        }

        public async Task<HtmlNode> UrlToNode(string url)
        {
            if (!TryPassRequestPerTimeLimit())
            {
                await Task.Delay(lastTimeRequestHappened + _maxPageRequestPerTime.period
                    - DateTime.Now);
            }
            return await _baseScraper.UrlToNode(url);
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

                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
