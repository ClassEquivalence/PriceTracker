using PriceTracker.Core.HelperUtils;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.LayersParallelBridge.UpsertionInvocation;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.LayersParallelBridge.Scraping
{

    public enum ScrapeState
    {
        Success, RequestsQuotaExhausted, UnallowedStatus
    }

    public class ParallelBridgeCitilinkScraper: IParallelBridgeCitilinkScraper
    {
        private readonly HttpClient _baseClient;
        private readonly ILogger? _logger;

        public ParallelBridgeCitilinkScraper(HttpClient client, ILogger? logger = null)
        {
            _baseClient = client;
            _logger = logger;
        }

        // Скрапинг + пост-определение результата
        public async Task<(string? scrapable, ScrapeState state, int requestsUsed)>
            ScrapeAsync(CitilinkScrapeTarget citilinkScrapeTarget, RequestsQuota quota)
        {
            CitilinkScrapeEpisodeDescriptor descriptor = citilinkScrapeTarget.CreateDescriptor();
            // Отправка запроса (+ соблюдение политик)
            var scrapeResult = await TryScrapeAsync(descriptor, quota);
            using var response = scrapeResult.response;

            // Логика верная, пока верно что:
            // response может быть null только если это запрещено политиками.
            // ПЕРЕСМОТРЕТЬ ЕСЛИ окажется по другому.
            if (response == null)
            {
                return (null, ScrapeState.RequestsQuotaExhausted, scrapeResult.requestsSpent);
            }

            _logger?.LogTrace($"ScrapeAsync responseStatus: {response.StatusCode}");

            // Проверка допустимости статусных кодов.
            if (!descriptor.AllowedStatuses.Contains(response.StatusCode))
                return new(null, ScrapeState.UnallowedStatus, scrapeResult.requestsSpent);

            var body = await response.Content.ReadAsStringAsync();
            return new(body, ScrapeState.Success, scrapeResult.requestsSpent);
        }

        /// <summary>
        /// Возвращает либо ответ с телом либо null, если получить ответ не удалось
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="quota"></param>
        /// <returns></returns>
        private async Task<(HttpResponseMessage? response, int requestsSpent)> TryScrapeAsync(
            CitilinkScrapeEpisodeDescriptor descriptor, RequestsQuota quota)
        {
            HttpResponseMessage? response = null;
            int requestsSpent = 0;
            while (response == null && quota.RequestsRemainingCount-- > 0)
            {
                response = await SendRequestAsync(descriptor);
                requestsSpent++;
            }
            return (response, requestsSpent);
        }

        /// <summary>
        /// Возвращает ответ в случае успеха, null в случае сбоя.
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage?> SendRequestAsync
            (CitilinkScrapeEpisodeDescriptor descriptor)
        {
            try
            {
                // Отправка запроса
                using ScrapeHttpRequest request = ScrapeHttpRequest.From(descriptor);
                var response = await _baseClient.SendAsync(request);
                return response;
            }
            catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException)
            {
                return null;
            }
        }
    }
}
