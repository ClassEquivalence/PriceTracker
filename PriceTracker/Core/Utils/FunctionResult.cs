
namespace PriceTracker.Core.Utils
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
