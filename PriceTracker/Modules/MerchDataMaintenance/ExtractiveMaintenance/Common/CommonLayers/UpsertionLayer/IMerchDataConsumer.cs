using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.UpsertionInvocationLayer;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.UpsertionLayer
{

    public interface IMerchDataConsumer<in ExecutionConstraints, MetaData>
        where ExecutionConstraints : LayersExecutionPolicies
        where MetaData : LayersExecutionMetaData
    {
        Task UpsertPortion(string url, ExecutionConstraints constraints);
    }
}
