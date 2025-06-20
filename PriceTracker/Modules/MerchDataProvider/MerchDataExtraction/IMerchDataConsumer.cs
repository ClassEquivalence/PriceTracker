using PriceTracker.Modules.MerchDataProvider.Models.ForParsing;

namespace PriceTracker.Modules.MerchDataProvider.MerchDataExtraction
{
    public interface IMerchDataConsumer<Dto>
        where Dto : MerchParsingDto
    {
        Task ReceiveAsync(IAsyncEnumerable<Dto> parsingDtos);
    }
}
