namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner
{
    public enum ScrapeCompletionState
    {
        Success,
        StoppedAsPolicyBudgetsReached,
        UnallowedStatus
    }
    public record CitilinkScrapeMetadata(ScrapeCompletionState CompletionState);
}
