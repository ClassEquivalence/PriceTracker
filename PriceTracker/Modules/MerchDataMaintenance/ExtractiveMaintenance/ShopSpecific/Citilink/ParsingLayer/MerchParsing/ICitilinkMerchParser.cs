using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;
using static PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.MerchParsing.CitilinkMerchParser;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.MerchParsing
{
    public interface ICitilinkMerchParser
    {
        Task<FunctionResult<IAsyncEnumerable<CitilinkMerchParsingDto>,
            RetreiveAllFromMerchCatalog_ExecState>> RetreiveAllFromMerchCatalog(BranchWithFunctionality catalog);
    }
}
