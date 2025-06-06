using PriceTracker.Models.DTOModels.ForParsing;
using PriceTracker.Models.Services.MerchDataExtraction.ExecutionState;
using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine;
using PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine.GUIExtractors.ShopSpecific.Citilink.ExtractionInstructions;
using System.Runtime.InteropServices;

namespace PriceTracker.Models.Services.MerchDataExtraction
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
