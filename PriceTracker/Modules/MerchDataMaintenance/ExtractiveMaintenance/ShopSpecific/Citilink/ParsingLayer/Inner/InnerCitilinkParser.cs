using PriceTracker.Core.HelperUtils;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner.Parsers;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner
{
    public class InnerCitilinkParser : IInnerCitilinkParser
    {
        private readonly ICitilinkParsableClassifier _classifier;

        private readonly ICitilinkCatalogsCatalogParser _catalogsCatalogParser;
        private readonly ICitilinkMerchCatalogParser _merchCatalogParser;
        private readonly ICitilinkRootCatalogParser _rootCatalogParser;

        public InnerCitilinkParser(ICitilinkParsableClassifier classifier,
            ICitilinkCatalogsCatalogParser catalogsCatalogParser,
            ICitilinkMerchCatalogParser merchCatalogParser,
            ICitilinkRootCatalogParser rootCatalogParser)
        {
            _classifier = classifier;

            _catalogsCatalogParser = catalogsCatalogParser;
            _merchCatalogParser = merchCatalogParser;
            _rootCatalogParser = rootCatalogParser;
        }
        /*
         public enum ParsableTypes { MerchCatalog, RootCatalog, CatalogsCatalog, MerchesJson, 
        IgnoredNode, Unknown }
         */
        public async Task<FunctionResult<CitilinkParsingSnapshot, CitilinkParseMetadata>>
            ParseUnitAsync(string parsable)
        {
            var type = _classifier.Classify(parsable);
            switch (type)
            {
                case ParsableType.MerchCatalog:
                    return ReturnForMerchCatalog();
                    break;
                case ParsableType.RootCatalog:
                    return await ReturnForRootCatalog(parsable);
                    break;
                case ParsableType.CatalogsCatalog:
                    return await ReturnForCatalogsCatalog(parsable);
                    break;
                case ParsableType.MerchesJson:
                    return await ReturnForMerchesJson(parsable);
                    break;
                case ParsableType.Unknown:
                    return ReturnForUnknown();
                    break;

                default:
                    goto case ParsableType.Unknown;
            }
        }
        private FunctionResult<CitilinkParsingSnapshot, CitilinkParseMetadata> ReturnForMerchCatalog()
        {
            var snapshot = new CitilinkParsingSnapshot() { Type = ParsableType.MerchCatalog };
            CitilinkParseMetadata metadata = new(ParseCompletionState.Success,
                ParsableType.MerchCatalog, []);
            return new(snapshot, metadata);
        }

        private async Task<FunctionResult<CitilinkParsingSnapshot, CitilinkParseMetadata>>
            ReturnForRootCatalog(string parsable)
        {
            var snapshot = new CitilinkParsingSnapshot()
            {
                Type = ParsableType.RootCatalog,
                DescendantNodeUrls = await _rootCatalogParser.ParseUnitAsync(parsable)
            };
            CitilinkParseMetadata metadata = new(ParseCompletionState.Success, ParsableType.RootCatalog);
            return new(snapshot, metadata);
        }

        private async Task<FunctionResult<CitilinkParsingSnapshot, CitilinkParseMetadata>>
            ReturnForCatalogsCatalog(string parsable)
        {
            var snapshot = new CitilinkParsingSnapshot()
            {
                Type = ParsableType.CatalogsCatalog,
                DescendantNodeUrls = await _catalogsCatalogParser.ParseUnitAsync(parsable)
            };
            CitilinkParseMetadata metadata = new(ParseCompletionState.Success, ParsableType.CatalogsCatalog);
            return new(snapshot, metadata);
        }

        private async Task<FunctionResult<CitilinkParsingSnapshot, CitilinkParseMetadata>>
            ReturnForMerchesJson(string parsable)
        {
            var snapshot = new CitilinkParsingSnapshot()
            {
                Type = ParsableType.MerchesJson,
                Merches = await _merchCatalogParser.ParseMerchesAsync(parsable),
                NodeMerchesCount = await _merchCatalogParser.ParseMerchCountAsync(parsable)
            };
            CitilinkParseMetadata metadata = new(ParseCompletionState.Success,
                ParsableType.MerchesJson);
            return new(snapshot, metadata);
        }

        private FunctionResult<CitilinkParsingSnapshot, CitilinkParseMetadata>
            ReturnForUnknown()
        {
            var snapshot = new CitilinkParsingSnapshot()
            {
                Type = ParsableType.Unknown,
            };
            CitilinkParseMetadata metadata = new(ParseCompletionState.Unsuccess_UnknownParsable,
                ParsableType.Unknown);
            return new(snapshot, metadata);
        }
    }
}
