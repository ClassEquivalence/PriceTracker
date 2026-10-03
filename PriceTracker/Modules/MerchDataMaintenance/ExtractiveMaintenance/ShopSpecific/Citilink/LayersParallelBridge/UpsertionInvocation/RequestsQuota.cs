namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.LayersParallelBridge.UpsertionInvocation
{
    public class RequestsQuota
    {
        public int RequestsRemainingCount;
        public RequestsQuota(int allowedRequestsCount)
        {
            RequestsRemainingCount = allowedRequestsCount;
        }
    }
}
