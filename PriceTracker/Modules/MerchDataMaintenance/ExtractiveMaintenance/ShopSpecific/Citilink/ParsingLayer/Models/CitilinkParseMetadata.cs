using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Models;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ParsingLayer.Models
{
    public enum ParseCompletionState
    {
        Success, Failure_PreviousLayer,
        Unsuccess_UnknownParsable
    }
    public class CitilinkParseMetadata
    {
        public CitilinkParseMetadata(ParseCompletionState state,
            ParsableType guessedType = ParsableType.Unknown,
            List<CitilinkScrapeTarget>? citilinkWebLocations = null)
        {
            State = state;
            GuessedType = guessedType;
            DiscoveredRelatedLocations = citilinkWebLocations ?? [];
        }

        public ParsableType GuessedType;
        public ParseCompletionState State;
        public List<CitilinkScrapeTarget> DiscoveredRelatedLocations;
    }
}
