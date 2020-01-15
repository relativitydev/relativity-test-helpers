using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Relativity.Tests.Helpers.Tests.Unit.MockHelpers
{
	public static class MockHttpMessageHandlerHelper
	{
		private static string _jsonBearer = @"{
    ""access_token"": ""@bearerToken"",
    ""expires_in"": 28800,
    ""token_type"": ""Bearer""
}";

		public static Mock<HttpMessageHandler> GetMockHttpMessageHandler(string bearerToken)
		{
			Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>();

			_jsonBearer = _jsonBearer.Replace("@bearerToken", bearerToken);
			mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
				.ReturnsAsync(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					Content = new StringContent(_jsonBearer)
				});

			return mockHttpMessageHandler;
		}
	}
}
