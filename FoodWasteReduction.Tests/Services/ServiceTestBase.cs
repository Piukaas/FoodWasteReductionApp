using System.Net;
using System.Text.Json;
using FoodWasteReduction.Web.Services.Interfaces;
using Moq;
using Moq.Protected;

namespace FoodWasteReduction.Tests.Services
{
    public abstract class ServiceTestBase
    {
        protected readonly Mock<IHttpClientFactory> HttpClientFactory;
        protected readonly Mock<HttpMessageHandler> MessageHandler;
        protected readonly Mock<IAuthGuardService> AuthGuardService;
        protected readonly HttpClient HttpClient;

        protected ServiceTestBase()
        {
            MessageHandler = new Mock<HttpMessageHandler>();
            AuthGuardService = new Mock<IAuthGuardService>();
            HttpClient = new HttpClient(MessageHandler.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            HttpClientFactory = new Mock<IHttpClientFactory>();
            HttpClientFactory.Setup(x => x.CreateClient("API")).Returns(HttpClient);
        }

        protected void SetupHttpResponse(HttpStatusCode statusCode, object? content = null)
        {
            var response = new HttpResponseMessage(statusCode);
            if (content != null)
            {
                response.Content = new StringContent(JsonSerializer.Serialize(content));
            }

            MessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);
        }

        protected void SetupUnauthorizedAccess()
        {
            AuthGuardService.Setup(x => x.GetToken()).Returns((string)null!);
            AuthGuardService.Setup(x => x.IsAuthenticated).Returns(false);

            MessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));
        }
    }
}
