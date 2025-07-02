



namespace PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine
{

    // TODO: Добавить исключение о досрочном завершении работы по каким либо причинам.

    // TODO: Чёт я натворил ерунды с дженериками.
    public class MerchExtractionCoordinator
    {

        private readonly List<MerchExtractionAgent> _extractionAgents;
        private readonly ILogger? _logger;

        public MerchExtractionCoordinator(List<MerchExtractionAgent> agents,
            ILogger? logger = null)
        {
            _logger = logger;
            _extractionAgents = agents;
        }


        /*
         TODO: Можно в двух методах ниже переписать, в первом цикле вызывая методы, во втором уже
        ожидая их. Чтобы асинхронность здесь имела смысл.
         */
        public async Task StartNewExtraction()
        {
            _logger?.LogTrace($"{nameof(MerchExtractionCoordinator)}: поочередно инициируются" +
                $"новые инстансы процессов извлечения товаров магазинов.");
            foreach (var agent in _extractionAgents)
            {
                await agent.StartNewExtraction();
            }
        }

        public async Task ContinuePreviousExtraction()
        {
            _logger?.LogTrace($"{nameof(MerchExtractionCoordinator)}: поочередно возобновляются" +
                $"процессы извлечения товаров магазинов.");
            foreach (var agent in _extractionAgents)
            {
                await agent.ContinueExtraction();
            }
        }
    }
}
