using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Upsertion
{
    public interface IInnerCitilinkMerchUpserter
    {
        Task EnqueueForUpsertionAsync(CitilinkMerchParsingDto dto);
        Task CompleteAsync();
    }
}
