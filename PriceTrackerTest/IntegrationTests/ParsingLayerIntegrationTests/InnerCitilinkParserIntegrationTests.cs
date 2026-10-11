using Microsoft.Extensions.Options;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner.Parsers;
using Xunit.Abstractions;

namespace PriceTrackerTest.IntegrationTests.ParsingLayerIntegrationTests
{
    public class InnerCitilinkParserIntegrationTests
    {
        private readonly ITestOutputHelper _loggerHelper;
        public InnerCitilinkParserIntegrationTests(ITestOutputHelper
            outputHelper)
        {
            _loggerHelper = outputHelper;
        }

        [Theory]
        [InlineData(@"TestData\01_05_2026\MerchesJson_izdeliya-is-bumagi.json")]
        public async Task ParseUnitAsync_ParsesMerchesCorrectly(string filePath)
        {
            string parsable = File.ReadAllText(filePath);

            CitilinkUpsertionOptions upsertionOptions = new()
            {
                CitilinkUrl = "https://www.citilink.ru/"
            };

            IOptions<CitilinkUpsertionOptions> interfacedOptions =
                Options.Create(upsertionOptions);

            ICitilinkParsableClassifier classifier = new CitilinkParsablesClassifier();
            ICitilinkCatalogsCatalogParser catalogsCatalogParser = new CitilinkCatalogsCatalogParser(interfacedOptions);
            ICitilinkRootCatalogParser rootCatalogParser = new CitilinkRootCatalogParser(interfacedOptions);
            ICitilinkMerchCatalogParser merchCatalogParser = new CitilinkMerchCatalogParser();
            var innerParser = new InnerCitilinkParser(classifier, catalogsCatalogParser,
                merchCatalogParser, rootCatalogParser);

            var parseRes = await innerParser.ParseUnitAsync(parsable);

            if (parseRes.Info.GuessedType != ParsableType.MerchesJson)
            {
                throw new InvalidOperationException();
            }

            foreach (var merch in parseRes.Result.Merches)
            {
                _loggerHelper.WriteLine($"Name: {merch.Name}, Price: {merch.Price},\n " +
                    $"CitilinkId: {merch.CitilinkId}");
            }
        }
    }
}
