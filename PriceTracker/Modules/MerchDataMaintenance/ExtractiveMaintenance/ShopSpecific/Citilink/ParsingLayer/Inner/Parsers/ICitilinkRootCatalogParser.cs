namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner.Parsers
{
    public interface ICitilinkRootCatalogParser
    {
        Task<List<string>> ParseUnitAsync(string parsable);
    }
}
