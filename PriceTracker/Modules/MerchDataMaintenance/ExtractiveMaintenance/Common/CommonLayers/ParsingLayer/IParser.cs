using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.ParsingLayer.Models;


namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.Common.CommonLayers.ParsingLayer
{
    public interface IParser<ExecutionConstraints, Data, ParsingMetaData>
        where Data : ParsingSnapshot
    {
        public Task<FunctionResult<Data?, ParsingMetaData>> ParseUnit(string parsableData,
            ExecutionConstraints parsingConstraints);
    }
}
