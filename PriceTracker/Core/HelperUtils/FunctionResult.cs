
namespace PriceTracker.Core.HelperUtils
{


    public class FunctionResult<Object, ExecutionInfo>
    {
        public FunctionResult(Object? result, ExecutionInfo info)
        {
            Result = result;
            Info = info;
        }
        public Object? Result { get; set; }
        public ExecutionInfo Info { get; set; }
    }
}
