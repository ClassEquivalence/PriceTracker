namespace PriceTracker.Core.Configuration.ProvidedWithDI
{
    public class ProductionDbOptions
    {
        public const string OptionsSectionKey = "ProductionDbOptions";
        public string ConnectionStringBody { get; set; }
        public string ConnectionPassword { get; set; }
    }
}
