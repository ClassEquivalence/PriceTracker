namespace PriceTracker.Core.Configuration.ProvidedWithDI
{
    public interface IAppEnvironment
    {
        string EnvironmentName { get; }
        bool IsDevelopment { get; }
        bool IsStaging { get; }
        bool IsProduction { get; }
    }
}
