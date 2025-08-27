using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ForParsing;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services
{
    public interface IMerchDataConsumer<Dto>
        where Dto : MerchParsingDto
    {
        Task Upsert(IAsyncEnumerable<Dto>
            complementaryData);
    }
}
