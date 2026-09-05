namespace PriceTracker.Core.Configuration.ProvidedWithDI.Options
{
    public class CitilinkUpsertionOptions
    {
        // Период обновления цен в днях
        public int CitilinkPriceUpdatePeriod { get; set; }


        // Максимально допустимое число запросов в рамках заданного временного промежутка. Запросом считается либо
        //полная загрузка html-документа со всеми его связями, либо действительно один запрос (как правило, весящий
        //достаточно много)
        public int MaxPageRequestsPerTime { get; set; }
        // Время указывается в часах.
        public float MinCooldownForPageRequests { get; set; }

        public string CitilinkMainCatalogUrl { get; set; } = string.Empty;
        public string CitilinkUrl { get; set; } = string.Empty;

        public string CitilinkAPIRoute { get; set; } = string.Empty;

        public string CitilinkHttpClientCookie { get; set; } = string.Empty;

        public string[] IgnoredCategorySlugs { get; set; } = [];
    }
}
