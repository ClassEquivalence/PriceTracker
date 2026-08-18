using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.Models.Node;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies;

namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner
{
    public class CitilinkScrapeAssignmentFactory : ICitilinkScrapeAssignmentFactory
    {
        private readonly ILogger? _logger;
        public CitilinkScrapeAssignmentFactory(ILogger? logger = null)
        {
            _logger = logger;
        }

        public CitilinkScrapeAssignment Create(CitilinkScrapeTarget target,
            ICitilinkScraperPoliciesEnforcer policiesEnforcer)
        {
            var descriptor = target.CreateDescriptor();

            _logger?.LogTrace($"{nameof(CitilinkScrapeAssignmentFactory)} " +
                $"{nameof(Create)}: descriptor has just been created:\n" +
                $"{descriptor.ToString()}");

            var assignment = new CitilinkScrapeAssignment(descriptor, policiesEnforcer);

            return assignment;
        }
    }
}
