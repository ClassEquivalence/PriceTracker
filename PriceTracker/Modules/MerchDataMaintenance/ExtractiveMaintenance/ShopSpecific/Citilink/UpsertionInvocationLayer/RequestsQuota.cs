namespace PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer
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
