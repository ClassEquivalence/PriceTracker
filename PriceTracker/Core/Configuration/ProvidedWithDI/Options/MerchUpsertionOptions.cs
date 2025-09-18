namespace PriceTracker.Core.Configuration.ProvidedWithDI.Options
{
    public class MerchUpsertionOptions
    {
        public const string OptionsSectionKey = "MerchUpsertionOptions";

        public CitilinkUpsertionOptions CitilinkUpsertionOptions { get; set; }

        public bool UpsertionActive { get; set; }


        public string UserAgent { get; set; } = string.Empty;
    }
}
