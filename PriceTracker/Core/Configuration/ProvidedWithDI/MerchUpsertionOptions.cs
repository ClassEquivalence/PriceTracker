namespace PriceTracker.Core.Configuration.ProvidedWithDI
{
    public class MerchUpsertionOptions
    {
        public const string OptionsSectionKey = "MerchUpsertionOptions";

        public CitilinkUpsertionOptions CitilinkUpsertionOptions { get; set; }

        public bool UpsertionActive { get; set; }
    }
}
