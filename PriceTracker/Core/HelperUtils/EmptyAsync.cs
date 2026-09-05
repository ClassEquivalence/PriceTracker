namespace PriceTracker.Core.HelperUtils
{
    public class EmptyAsync
    {
        public static async IAsyncEnumerable<Something> GetEmptyAsyncEnumerable<Something>()
        {
            yield break;
        }
    }
}
