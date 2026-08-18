using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.DataSynchronizationLayer.Pipeline
{
    public interface ICitilinkDataAugmenter
    {

        Task<CitilinkExecutionMetadata>
            AugmentNodeDataAsync(CitilinkNode node, ICitilinkScraperPoliciesEnforcer enforcer);
        Task CompleteAsync();
    }
}
