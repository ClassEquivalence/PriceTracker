namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner
{


    public interface ICitilinkParsableClassifier
    {
        ParsableType Classify(string parsable);
    }
}
