using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ParsingLayer.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.UpsertionLayer
{
    public interface IMerchDataConsumer
    {
    }

    public interface IMerchDataConsumer<Dto>: IMerchDataConsumer
        where Dto : MerchParsingDto
    {
        Task Upsert(IAsyncEnumerable<Dto>
            complementaryData);
    }
}
