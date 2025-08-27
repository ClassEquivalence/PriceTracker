using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;

namespace PriceTracker.Core.Utils
{
    public class EmptyAsync
    {
        public static async IAsyncEnumerable<Something> GetEmptyAsyncEnumerable<Something>()
        {
            yield break;
        }
    }
}
