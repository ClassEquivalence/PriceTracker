using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;

namespace PriceTrackerTest.UnitTests.Modules.MerchDataMaintenance.ParsingLayer;

public class CitilinkParsablesClassifierTests
{
    [Theory]
    [InlineData(@"TestData\01_05_2026\RootCatalog.html", ParsableType.RootCatalog)]
    [InlineData(@"TestData\01_05_2026\MerchCatalog_ezhednevniki-i-zapisnye-knizhki.html", ParsableType.MerchCatalog)]
    [InlineData(@"TestData\01_05_2026\MerchesJson_izdeliya-is-bumagi.json", ParsableType.MerchesJson)]
    [InlineData(@"TestData\01_05_2026\CatalogsCatalog_izdeliya-iz-bumagi.html", ParsableType.CatalogsCatalog)]
    public void Classify_ClassifiesCorrectly(string filePath, ParsableType expectedType)
    {
        string parsable = File.ReadAllText(filePath);
        CitilinkParsablesClassifier classifier = new CitilinkParsablesClassifier();
        var obtainedType = classifier.Classify(parsable);

        Assert.Equal(expectedType, obtainedType);
    }
}
