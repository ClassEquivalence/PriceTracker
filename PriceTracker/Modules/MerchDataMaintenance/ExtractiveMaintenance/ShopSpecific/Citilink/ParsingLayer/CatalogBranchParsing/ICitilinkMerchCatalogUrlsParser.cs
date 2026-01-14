using PriceTracker.Core.Models.Process.ShopSpecific.Citilink.ExtractionState.CatalogTree;
using PriceTracker.Core.Utils;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.CatalogBranchParsing
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
