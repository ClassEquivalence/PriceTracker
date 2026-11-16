namespace PriceTracker.Core.HelperUtils
{
    public static class WaitUntilTime
    {
        public static async Task WaitUntil(DateTime timeToWaitUntil) 
        {
            await Task.Delay(timeToWaitUntil-DateTime.Now);
        }
    }
}
