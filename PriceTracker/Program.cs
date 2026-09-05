using PriceTracker.Core.App;
namespace PriceTracker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new AppBuilder();
            var app = builder.BuildDevelopmentStageApp();
            app.Initialize();
            var appTask = app.RunAsync();
            await appTask;
        }
    }
}
