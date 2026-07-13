using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.CoordinationLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer
{

    public enum PartialCycleState
    {
        PartialCompleted,
        FullCompleted,
        StopRequested
    }
    public enum PartialCycleExecutionMode
    {
        StartAnew,
        Continue,
        AutoChoice
    }
    public interface ICitilinkUpsertionInvoker
    {
        Task<PartialCycleState> ExecutePartialCycleAsync(CitilinkEnforcersSharedContext cycleQuota,
            PartialCycleExecutionMode mode);
        Task StopPartialCycleAsync();
    }
}
