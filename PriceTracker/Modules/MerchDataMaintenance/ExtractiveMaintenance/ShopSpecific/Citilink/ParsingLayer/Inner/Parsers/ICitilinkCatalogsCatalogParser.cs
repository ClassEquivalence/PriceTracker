namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner.Parsers
{
    public interface ICitilinkCatalogsCatalogParser
    {
        Task<List<string>> ParseUnitAsync(string parsable);
    }
}
