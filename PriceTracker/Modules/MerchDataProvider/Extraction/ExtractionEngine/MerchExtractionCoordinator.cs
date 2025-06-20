



namespace PriceTracker.Modules.MerchDataProvider.Extraction.ExtractionEngine
{

    // TODO: Добавить исключение о досрочном завершении работы по каким либо причинам.

    // TODO: Чёт я натворил ерунды с дженериками.
    public class MerchExtractionCoordinator
    {

        private readonly List<MerchExtractionAgent> _extractionAgents;

        public MerchExtractionCoordinator(List<MerchExtractionAgent> agents)
        {
            _extractionAgents = agents;
        }


        /*
         TODO: Можно в двух методах ниже переписать, в первом цикле вызывая методы, во втором уже
        ожидая их. Чтобы асинхронность здесь имела смысл.
         */
        public async Task StartNewExtraction()
        {
            foreach (var agent in _extractionAgents)
            {
                await agent.StartNewExtraction();
            }
        }

        public async Task ContinuePreviousExtraction()
        {
            foreach (var agent in _extractionAgents)
            {
                await agent.ContinueExtraction();
            }
        }
    }
}
