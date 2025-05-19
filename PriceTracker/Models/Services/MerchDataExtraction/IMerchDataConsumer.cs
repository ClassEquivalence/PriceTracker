using PriceTracker.Models.DTOModels.ForParsing;

namespace PriceTracker.Models.Services.MerchDataExtraction
{
    public interface IMerchDataConsumer<Dto>
        where Dto : MerchParsingDto
    {
        Task ReceiveAsync(IAsyncEnumerable<Dto> parsingDtos);
    }
}
