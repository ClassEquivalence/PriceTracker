using PriceTracker.Core.Models.Domain;
using PriceTracker.Modules.MerchDataProvider.Models.ForParsing;

namespace PriceTracker.Modules.MerchDataProvider.Upsert
{
    public abstract class MerchDataUpserter<CoreDto, ExtractionDto>
        where CoreDto : MerchDto where ExtractionDto : MerchParsingDto
    {
        protected abstract Task Upsert(IAsyncEnumerable<ExtractionDto>
            complementaryData);
    }
}
