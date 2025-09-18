namespace PriceTracker.Core.Configuration.ProvidedWithDI
{
    public class HostEnvironmentAdapter: IAppEnvironment
    {
        private readonly IHostEnvironment _hostEnv;

        public HostEnvironmentAdapter(IHostEnvironment hostEnv)
        {
            _hostEnv = hostEnv;
        }
        public string EnvironmentName => _hostEnv.EnvironmentName;
        public bool IsDevelopment => _hostEnv.IsDevelopment();
        public bool IsStaging => _hostEnv.IsStaging();
        public bool IsProduction => _hostEnv.IsProduction();
    }
}
