using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.ParsingLayer.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.UpsertionInvocationLayer;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.ParsingLayer
{



    public interface IMerchDataExtractor<in ExecutionConstraints, MetaData, Data>
        where ExecutionConstraints : LayersExecutionPolicies
        where MetaData : LayersExecutionMetaData
        where Data : ParsingSnapshot
    {

        public Task<FunctionResult<Data?, MetaData>> ParseUnit(string url, ExecutionConstraints constraints);

    }
}
