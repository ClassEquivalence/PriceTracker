using Moq;
using Moq.Protected;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.ScrapingLayer.Inner;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies;
using PriceTracker.Modules.MerchDataMaintenance.ExtractiveMaintenance.ShopSpecific.Citilink.UpsertionInvocationLayer.Models.Policies.PolicyEnforcersProvider;
using System.Net;

namespace PriceTrackerTest.UnitTests.Modules.MerchDataMaintenance.ScrapingLayer
{
    public class InnerCitilinkScraperTests
    {

        public InnerCitilinkScraperTests()
        {

        }

        public static HttpClient MakeMockableClient(Action<IProtectedMock<HttpMessageHandler>> mockSetup)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            mockSetup(httpMessageHandlerMock.Protected());

            var httpHandler = httpMessageHandlerMock.Object;
            var httpClient = new HttpClient(httpHandler);
            return httpClient;
        }


        // Поведение ретраев?
        // Должен остановить работу и выдать спец резалт после неудачной серии ретраев
        [Theory]
        [InlineData("https://learn.microsoft.com/en-us/dot")]
        public async Task ScrapeAsync_ReturnsRetryLimitOnFail(string uriString)
        {
            var httpClient = MakeMockableClient(pm =>
            {
                pm.Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new TaskCanceledException());
            });
            var innerScraper = new NewCitilinkScraper(httpClient);

            CitilinkEnforcersSharedContext sharedContext = new(); // or 1, 1
            CitilinkScrapeAssignment assignment = new(uriString, HttpMethod.Get, 
                [HttpStatusCode.OK], sharedContext);
            var scrapeRes = await innerScraper.ScrapeAsync(assignment);

            Assert.Equal(ScrapeCompletionState.StoppedAsPolicyBudgetsReached,
                scrapeRes.Info.CompletionState);
            Assert.Null(scrapeRes.Result);
        }

        // Поведение ретраев?
        // Должен остановить работу и выдать спец резалт после неудачной серии ретраев
        // TODO: можно объеденить с другой проверкой боттлнека, сделав в 1 месте 3 переменные
        // ... или даже больше 3х.
        // 1: maxPerScrapeEpisode, 2: maxPerPartialCycle, 3: expectedAttempts
        [Theory]
        [InlineData(5)]
        public async Task ScrapeAsync_RequestsBottleneckedByScrapeEpisodeCounter(int attempts)
        {
            int requestsCounter = 0;

            string uriString = "https://learn.microsoft.com/en-us/dot";
            var httpClient = MakeMockableClient(pm =>
            {
                pm.Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Returns(() =>
                {
                    requestsCounter++;
                    throw new TaskCanceledException();
                });
            });
            var innerScraper = new NewCitilinkScraper(httpClient);

            CitilinkEnforcersSharedContext sharedContext = new(attempts, attempts*2); // or 1, 1
            CitilinkScrapeAssignment assignment = new(uriString, HttpMethod.Get,
                [HttpStatusCode.OK], sharedContext);

            var scrapeRes = await innerScraper.ScrapeAsync(assignment);

            Assert.Equal(ScrapeCompletionState.StoppedAsPolicyBudgetsReached,
                scrapeRes.Info.CompletionState);
            Assert.Equal(attempts, requestsCounter);
        }

        [Theory]
        [InlineData(5)]
        public async Task ScrapeAsync_RequestsBottleneckedByPartialCycleCounter(int attempts)
        {
            int requestsCounter = 0;

            string uriString = "https://learn.microsoft.com/en-us/dot";
            var httpClient = MakeMockableClient(pm =>
            {
                pm.Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Returns(() =>
                {
                    requestsCounter++;
                    throw new TaskCanceledException();
                });
            });
            var innerScraper = new NewCitilinkScraper(httpClient);

            CitilinkEnforcersSharedContext sharedContext = new(attempts*2, attempts); // or 1, 1
            CitilinkScrapeAssignment assignment = new(uriString, HttpMethod.Get,
                [HttpStatusCode.OK], sharedContext);

            var scrapeRes = await innerScraper.ScrapeAsync(assignment);

            Assert.Equal(ScrapeCompletionState.StoppedAsPolicyBudgetsReached,
                scrapeRes.Info.CompletionState);
            Assert.Equal(attempts, requestsCounter);
        }



        [Theory]
        [InlineData(HttpStatusCode.OK, new HttpStatusCode[] { HttpStatusCode.OK }, 
            ScrapeCompletionState.Success)]
        [InlineData(HttpStatusCode.RequestTimeout, new HttpStatusCode[] { HttpStatusCode.OK },
            ScrapeCompletionState.UnallowedStatus)]
        public async Task ScrapeAsync_ReturnsSuccess(HttpStatusCode responseCode,
            HttpStatusCode[] allowedStatusCodes, ScrapeCompletionState expectedState)
        {

            string uriString = "https://learn.microsoft.com/en-us/dot";
            var httpClient = MakeMockableClient(pm =>
            {
                pm.Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(responseCode));
            });
            var innerScraper = new NewCitilinkScraper(httpClient);

            CitilinkEnforcersSharedContext sharedContext = new(); // or 1, 1
            CitilinkScrapeAssignment assignment = new(uriString, HttpMethod.Get,
                allowedStatusCodes.ToList(), sharedContext);

            var scrapeRes = await innerScraper.ScrapeAsync(assignment);
            Assert.Equal(expectedState, scrapeRes.Info.CompletionState);
        }
    }
}
