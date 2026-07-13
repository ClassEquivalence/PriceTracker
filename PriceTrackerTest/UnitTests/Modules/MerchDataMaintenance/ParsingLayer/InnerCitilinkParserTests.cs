using Microsoft.Extensions.Options;
using PriceTracker.Core.Configuration.ProvidedWithDI.Options;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner.Parsers;

namespace PriceTrackerTest.UnitTests.Modules.MerchDataMaintenance.ParsingLayer
{
    public class InnerCitilinkParserTests
    {
        // Количество, структурную корректность, несколько ключевых элементов,
        // нужны ли корнер-кейсы?

        // count: 17 12 10 14 6
        // 14 10 11 7 6
        // 10 14 15 11 7
        // 11 6
        // 59 + 48 + 57 + 17 = 181

        // MerchCatalog - не запоминает число мерчей. Плохое непредсказуемое поведение?
        [Theory]
        [InlineData(@"TestData\01_05_2026\RootCatalog.html", 181)]
        [InlineData(@"TestData\01_05_2026\MerchCatalog_ezhednevniki-i-zapisnye-knizhki.html", 0)]
        [InlineData(@"TestData\01_05_2026\MerchesJson_izdeliya-is-bumagi.json", 36)]
        [InlineData(@"TestData\01_05_2026\CatalogsCatalog_izdeliya-iz-bumagi.html", 9)]
        public async Task ParseUnitAsync_ParsablesCountFoundCorrectly(string filePath, int expectedCount)
        {
            string parsable = File.ReadAllText(filePath);

            CitilinkUpsertionOptions upsertionOptions = new()
            {
                CitilinkUrl = "https://www.citilink.ru/"
            };

            IOptions<CitilinkUpsertionOptions> interfacedOptions =
                Options.Create<CitilinkUpsertionOptions>(upsertionOptions);

            ICitilinkParsableClassifier classifier = new CitilinkParsablesClassifier();
            ICitilinkCatalogsCatalogParser catalogsCatalogParser = new CitilinkCatalogsCatalogParser(interfacedOptions);
            ICitilinkRootCatalogParser rootCatalogParser = new CitilinkRootCatalogParser(interfacedOptions);
            ICitilinkMerchCatalogParser merchCatalogParser = new CitilinkMerchCatalogParser();
            var innerParser = new InnerCitilinkParser(classifier, catalogsCatalogParser,
                merchCatalogParser, rootCatalogParser);

            var parseRes = await innerParser.ParseUnitAsync(parsable);
            var type = parseRes.Info.GuessedType;
            int descendantNodeCount;
            switch (type)
            {
                case ParsableType.MerchCatalog:
                    descendantNodeCount = parseRes?.Result?.Merches?.Count ?? 0;
                    break;
                case ParsableType.RootCatalog or ParsableType.CatalogsCatalog:
                    descendantNodeCount = parseRes?.Result?.DescendantNodeUrls?.Count ?? 0;
                    break;
                case ParsableType.MerchesJson:
                    descendantNodeCount = parseRes?.Result?.Merches?.Count ?? 0;
                    break;
                case ParsableType.Unknown:
                    descendantNodeCount = 0;
                    break;
                default:
                    descendantNodeCount = 0;
                    break;
            }

            Assert.Equal(expectedCount, descendantNodeCount);
        }


        [Theory]
        [InlineData(@"TestData\01_05_2026\RootCatalog.html", ParsableType.RootCatalog)]
        [InlineData(@"TestData\01_05_2026\MerchCatalog_ezhednevniki-i-zapisnye-knizhki.html", ParsableType.MerchCatalog)]
        [InlineData(@"TestData\01_05_2026\MerchesJson_izdeliya-is-bumagi.json", ParsableType.MerchesJson)]
        [InlineData(@"TestData\01_05_2026\CatalogsCatalog_izdeliya-iz-bumagi.html", ParsableType.CatalogsCatalog)]
        public async Task ParseUnitAsync_GuessesTypesCorrectly(string filePath, ParsableType guessedType)
        {
            string parsable = File.ReadAllText(filePath);

            

            CitilinkUpsertionOptions upsertionOptions = new()
            {
                CitilinkUrl = "https://www.citilink.ru/"
            };

            IOptions<CitilinkUpsertionOptions> interfacedOptions = 
                Options.Create<CitilinkUpsertionOptions>(upsertionOptions);

            ICitilinkParsableClassifier classifier = new CitilinkParsablesClassifier();
            ICitilinkCatalogsCatalogParser catalogsCatalogParser = new CitilinkCatalogsCatalogParser(interfacedOptions);
            ICitilinkRootCatalogParser rootCatalogParser = new CitilinkRootCatalogParser(interfacedOptions);
            ICitilinkMerchCatalogParser merchCatalogParser = new CitilinkMerchCatalogParser();
            var innerParser = new InnerCitilinkParser(classifier, catalogsCatalogParser,
                merchCatalogParser, rootCatalogParser);

            var parseRes = await innerParser.ParseUnitAsync(parsable);
            var type = parseRes.Info.GuessedType;

            Assert.Equal(guessedType, type);
        }


        
    }
}
