using Moq;
using Moq.Protected;

namespace PriceTrackerTest.UnitTests
{
    public class HttpClientTests
    {
        [Fact]
        public async void SendAsync_HttpExceptionTransferableFromHttpMessageHandler()
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Throws(new HttpRequestException());

            var httpHandler = httpMessageHandlerMock.Object;
            HttpClient client = new HttpClient(httpHandler);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "https://learn.microsoft.com/en-us/dot");

            await Assert.ThrowsAsync<HttpRequestException>(async () => { await client.SendAsync(httpRequest); });
        }

        [Fact]
        public async void SendAsync_TaskCancelledTransferableFromHttpMessageHandler()
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Throws(new TaskCanceledException());

            var httpHandler = httpMessageHandlerMock.Object;
            HttpClient client = new HttpClient(httpHandler);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "https://learn.microsoft.com/en-us/dot");

            await Assert.ThrowsAsync<TaskCanceledException>(async () => { await client.SendAsync(httpRequest); });
        }

    }
}
