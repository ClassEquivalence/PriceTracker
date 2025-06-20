public static class Configs
{
    public static readonly TimeSpan PriceUpdatePeriod = TimeSpan.FromDays(7);
    public static readonly int DefaultItemTestId = 1;
    public static readonly string DefaultNameForItems = "Undefined";

    // TODO: Уточнить на что именно влияет задержка.
    /// <summary>
    /// Задержка действий браузера в миллисекундах.
    /// </summary>
    public static readonly (float minDelay, float maxDelay) 
        HeadlessBrowserDelayRange = (15000, 30000);
}