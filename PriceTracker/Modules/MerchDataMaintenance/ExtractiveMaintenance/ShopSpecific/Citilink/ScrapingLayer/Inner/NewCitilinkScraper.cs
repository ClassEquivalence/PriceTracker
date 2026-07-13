using PriceTracker.Core.Utils;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner
{
    public class NewCitilinkScraper : IInnerCitilinkScraper
    {
        private readonly HttpClient _baseClient;
        private readonly ILogger? _logger;

        public NewCitilinkScraper(HttpClient client, ILogger? logger = null)
        {
            _baseClient = client;
            _logger = logger;
        }

        // Скрапинг + пост-определение результата
        public async Task<FunctionResult<string?, CitilinkScrapeMetadata>>
            ScrapeAsync(CitilinkScrapeAssignment assignment)
        {
            var descriptor = assignment.ScrapeEpisodeDescriptor;
            var policiesEnforcer = assignment.PoliciesEnforcer;

            // Отправка запроса (+ соблюдение политик)
            using var response = await TryScrapeAsync(assignment);

            // Логика верная, пока верно что:
            // response может быть null только если это запрещено политиками.
            // ПЕРЕСМОТРЕТЬ ЕСЛИ окажется по другому.
            if (response == null)
            {
                return new(null, new(ScrapeCompletionState.StoppedAsPolicyBudgetsReached));
            }

            _logger?.LogTrace($"ScrapeAsync responseStatus: {response.StatusCode}");

            // Проверка допустимости статусных кодов.
            if (!descriptor.AllowedStatuses.Contains(response.StatusCode))
                return new(null, new(ScrapeCompletionState.UnallowedStatus));

            var body = await response.Content.ReadAsStringAsync();
            return new(body, new(ScrapeCompletionState.Success));
        }


        private async Task<HttpResponseMessage?> TryScrapeAsync(
            CitilinkScrapeAssignment assignment)
        {
            var descriptor = assignment.ScrapeEpisodeDescriptor;
            var policiesEnforcer = assignment.PoliciesEnforcer;
            HttpResponseMessage? response = null;
            while (response == null && policiesEnforcer.TryRegisterScrapeRequest())
            {
                response = await SendRequestAsync(descriptor);
            }
            return response;
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


