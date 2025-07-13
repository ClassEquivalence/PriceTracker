using PriceTracker.Modules.MerchDataUpserter.Core.Models.ForParsing;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services
{
    public interface IMerchDataConsumer<Dto>
        where Dto : MerchParsingDto
    {
        Task Upsert(IAsyncEnumerable<Dto>
            complementaryData);
    }
}
