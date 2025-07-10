using PriceTracker.Modules.MerchDataUpserter.Core.Models.ForParsing;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion
{
    public interface IMerchDataConsumer<Dto>
        where Dto : MerchParsingDto
    {
        Task Upsert(IAsyncEnumerable<Dto>
            complementaryData);
    }
}
