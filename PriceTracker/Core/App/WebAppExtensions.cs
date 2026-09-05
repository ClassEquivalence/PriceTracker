using PriceTracker.Modules.Repository;

namespace PriceTracker.Core.App
{
    public static class WebAppExtensions
    {
        public static void Initialize(this WebApplication app)
        {
            app.Services.GetService<RepositoryInitializer>()?.EnsureInitialized();
        }
    }
}
