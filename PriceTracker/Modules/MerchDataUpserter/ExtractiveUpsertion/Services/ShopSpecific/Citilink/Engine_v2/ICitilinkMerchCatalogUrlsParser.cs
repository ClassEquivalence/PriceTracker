using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Models.ShopSpecific.Citilink;

namespace PriceTracker.Modules.MerchDataUpserter.ExtractiveUpsertion.Services.ShopSpecific.Citilink.Engine_v2
{
    public interface ICitilinkMerchCatalogUrlsParser
    {
        enum GetUrlsPortion_Info
        {
            Success,
            ServerTired,
            NoUnprocessedBranchesLeft,
            Error
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// Ненулевой список, только если Result.Info = Success
        /// </returns>
        public Task<FunctionResult<List<BranchWithFunctionality>?, GetUrlsPortion_Info>> GetMerchCatalogUrlsPortion();

    }
}
