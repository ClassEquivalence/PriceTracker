using PriceTracker.Models.DTOModels.ForParsing;
using PriceTracker.Models.Services.MerchDataExtraction.ExtractionInstructions;

namespace PriceTracker.Models.Services.MerchDataExtraction.MerchExtractionEngine
{

    /// <summary>
    /// Не стоит реализовывать этот интерфейс. Стоит реализовывать обобщенный вариант.
    /// </summary>
    public interface IMerchDataExtractor { }

    public interface IMerchDataExtractor<Dto, ExtractionProcessInfo>: IMerchDataExtractor
        where Dto: MerchParsingDto where ExtractionProcessInfo: ExtractionAgentExecutionStateInfo
    {
        /// <summary>
        /// Если передана extractionData - процесс продолжается с момента в ней.
        /// Если не передана - процесс начинается с нуля.
        /// </summary>
        /// <param name="extractionData"></param>
        /// <returns></returns>
        IAsyncEnumerable<Dto> RunExtractionProcess(ExtractionProcessInfo? extractionData = null);
        ExtractionProcessInfo? GetProgress();
    }
}
