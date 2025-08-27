public static class Configs
{
    /// <summary>
    /// Период обновления цен. Период циклов апсершна цен и товаров.
    /// </summary>
    public static readonly TimeSpan PriceUpdatePeriod = TimeSpan.FromDays(7);

    public static readonly int DefaultItemTestId = 1;
    public static readonly string DefaultNameForItems = "Undefined";

    // TODO: Уточнить на что именно влияет задержка.
    /// <summary>
    /// Задержка действий браузера в секундах.
    /// </summary>
    public static readonly (float minDelay, float maxDelay)
        HeadlessBrowserDelayRange = (15, 30);

    public static readonly (int requests, TimeSpan period) MaxPageRequestsPerTime = (300, TimeSpan.FromHours(12));

    public static string CitilinkMainCatalogUrl = "https://www.citilink.ru/catalog/";
    public static string CitilinkUrl = "https://www.citilink.ru/";

    public static string CitilinkAPIRoute = $"{CitilinkUrl}graphql/";
}