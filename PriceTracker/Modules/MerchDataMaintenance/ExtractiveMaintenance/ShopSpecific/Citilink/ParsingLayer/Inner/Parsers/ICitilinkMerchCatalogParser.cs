using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner.Parsers
{
    public interface ICitilinkMerchCatalogParser
    {
        Task<List<CitilinkMerchParsingDto>> ParseMerchesAsync(string parsableJson);
        public Task<int> ParseMerchCountAsync(string parsableJson);
    }
}
