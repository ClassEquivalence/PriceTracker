namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.CoordinationLayer
{
    public class CitilinkCoordinationPolicies
    {
        public CitilinkCoordinationPolicies(int maxRequestsPerPartialCycle)
        {
            MaxRequestsPerPartialCycle = maxRequestsPerPartialCycle;
        }
        public int MaxRequestsPerPartialCycle;
    }
}
