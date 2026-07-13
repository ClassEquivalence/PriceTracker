using Moq;
using Moq.Protected;

namespace PriceTrackerTest.IntegrationTests
{
    public class HttpClientTests
    {

        [Theory]
        [InlineData(101)]
        public async void SendAsync_ThrowsTaskCancelledExceptionOnTimeout(int secondsDelay)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Returns(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(secondsDelay));
                    return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                });

            var httpHandler = httpMessageHandlerMock.Object;
            HttpClient client = new HttpClient(httpHandler);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "https://learn.microsoft.com/en-us/dot");

            await Assert.ThrowsAsync<TaskCanceledException>(async () => { await client.SendAsync(httpRequest); });
        }

    }
}
