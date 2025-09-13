using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;
using static PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.MerchParser.CitilinkMerchParser;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2.MerchParser
{
    public interface ICitilinkMerchParser
    {
        Task<FunctionResult<IAsyncEnumerable<CitilinkMerchParsingDto>,
            RetreiveAllFromMerchCatalog_ExecState>> RetreiveAllFromMerchCatalog(BranchWithFunctionality catalog);
    }
}
