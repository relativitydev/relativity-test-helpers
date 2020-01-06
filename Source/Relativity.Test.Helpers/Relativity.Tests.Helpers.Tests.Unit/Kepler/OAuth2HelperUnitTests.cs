using kCura.Relativity.Client;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Relativity.Services.Security;
using Relativity.Services.Security.Models;
using Relativity.Test.Helpers.Kepler;
using Relativity.Tests.Helpers.Tests.Unit.MockHelpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Relativity.Tests.Helpers.Tests.Unit.Kepler
{
	[TestFixture]
	public class OAuth2HelperUnitTests
	{
		private Mock<IOAuth2ClientManager> _mockOAuth2ClientManager;
		private Mock<IRSAPIClient> _mockRsapiClient;
		private Mock<HttpMessageHandler> _mockHttpMessageHandler;
		private IOAuth2Helper Sut;

		private const string BearerToken =
			"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IndrdTdRcWd5NTRXbGM4eW1weVY5NzJlRnpSVSIsImtpZCI6IkMyNEJCQjQyQTgzMkU3ODVBNTczQ0NBNkE3MjU3REVGNjc4NUNEMTUifQ.eyJpc3MiOiJSZWxhdGl2aXR5IiwiYXVkIjoiUmVsYXRpdml0eS9yZXNvdXJjZXMiLCJleHAiOjE1NzgzNTEwNjAsIm5iZiI6MTU3ODMyMjI2MCwiY2xpZW50X2lkIjoiOWExMmVjMDdiZDg5MzVhMzM4NTNhOWYyZDciLCJvcl9sYiI6IlRydWUiLCJyZWxfaW5zIjoiQThGNkI2NjItRTFCQi00QTYzLTk1NEUtMDFCOEFCNjVCMDM0IiwicmVsX3VhaSI6IjkiLCJyZWxfdWZuIjoiUmVsYXRpdml0eSIsInJlbF91bG4iOiJBZG1pbiIsInJlbF91biI6InJlbGF0aXZpdHkuYWRtaW5AcmVsYXRpdml0eS5jb20iLCJzY29wZSI6IlN5c3RlbVVzZXJJbmZvIiwicmVsX29yaWdpbiI6IjE3Mi4yNS4xMzguMTYxIn0.jkcYA_1qXduGkrNUoO09P-WwGLjfvzhEZ3NUx07loColQWDhhU7yWPNSyxgLzTRBb5NLDM3qWzuuJoXy-NKcdeDYFC8VMZgE-IRR2zTRUbOJFuj2gGdFuEEIBnLik6_80lyvWdVcNHBybFt35KqFy9tvpkboXxciUlFFkA2Y-JAgzJYzxHj7bmNw6Q_0uxQLZ75Vg_lgw80kJokBQ5Dc-3oICMbbu5pS5-YNE3iz6BlcW5FC4Cm-291CJI_QUV6hgK3pk4KygPQTsunHWM60nQSbh7zguc2N9aJnx9lXWEQ6tOKpc-xXKJVgRxH9B6BdOf93V31Jy92udSz2f6AL6w";

		private string _jsonBearer = @"{
    ""access_token"": ""@bearerToken"",
    ""expires_in"": 28800,
    ""token_type"": ""Bearer""
}";

		[OneTimeSetUp]
		public void SetUp()
		{
			_mockOAuth2ClientManager = GetMockOAuth2ClientManager();
			_mockRsapiClient = GetMockRsapiClient();
			_mockHttpMessageHandler = GetMockHttpMessageHandler();

			Sut = new OAuth2Helper(_mockOAuth2ClientManager.Object, _mockRsapiClient.Object);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public async Task CreateOAuth2ClientAsyncTest()
		{
			// Arrange
			string username = "test@test.com";
			string oAuth2Name = "OAuth2Name";

			// Act
			Services.Security.Models.OAuth2Client result = await Sut.CreateOAuth2ClientAsync(username, oAuth2Name);

			// Assert
			Assert.IsTrue(result.Id.Equals("OAuth2Id"));
		}

		[Test]
		public void DeleteOAuth2ClientAsyncTest()
		{
			// Arrange
			string clientId = "1";

			// Act

			// Assert
			Assert.DoesNotThrow(() => Sut.DeleteOAuth2ClientAsync(clientId).Wait());
		}

		[Test]
		public async Task ReadOAuth2ClientAsyncTest()
		{
			// Arrange
			string oAuth2Name = "OAuth2Name";

			// Act
			Services.Security.Models.OAuth2Client result = await Sut.ReadOAuth2ClientAsync(oAuth2Name);

			// Assert
			Assert.IsTrue(result.Id.Equals("OAuth2Id"));
		}

		[Test]
		public async Task DoesOAuth2ClientExistAsyncTest()
		{
			// Arrange
			string oAuth2Name = "OAuth2Name";

			// Act
			bool result = await Sut.DoesOAuth2ClientExistAsync(oAuth2Name);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public async Task GetBearerTokenAsyncTest()
		{
			// Arrange
			string protocol = "http";
			string serverAddress = "172.99.99.99";
			string clientId = "";
			string clientSecret = "";
			string scope = "";
			string grantType = "";

			// Act
			string result = await Sut.GetBearerTokenAsync(protocol, serverAddress, clientId, clientSecret, scope, grantType, _mockHttpMessageHandler.Object);

			// Assert
			Assert.IsTrue(result.Equals(BearerToken));
		}

		private Mock<IOAuth2ClientManager> GetMockOAuth2ClientManager()
		{
			Mock<IOAuth2ClientManager> mockOAuth2ClientManager = new Mock<IOAuth2ClientManager>();
			Services.Security.Models.OAuth2Client oAuth2Client = new Services.Security.Models.OAuth2Client()
			{
				Id = "OAuth2Id",
				Name = "OAuth2Name",
				Secret = "OAuth2Secret",
				ContextUser = 999
			};
			List<Services.Security.Models.OAuth2Client> oAuth2Clients = new List<Services.Security.Models.OAuth2Client>() { oAuth2Client };

			mockOAuth2ClientManager
				.Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<OAuth2Flow>(), It.IsAny<IEnumerable<Uri>>(), It.IsAny<int>()))
				.Returns(Task.FromResult(oAuth2Client));

			mockOAuth2ClientManager
				.Setup(x => x.DeleteAsync(It.IsAny<string>()))
				.Returns(Task.CompletedTask);

			mockOAuth2ClientManager
				.Setup(x => x.ReadAllAsync())
				.Returns(Task.FromResult(oAuth2Clients));

			return mockOAuth2ClientManager;
		}

		private Mock<IRSAPIClient> GetMockRsapiClient()
		{
			Mock<IRSAPIClient> mockRsapiClient = new Mock<IRSAPIClient>();

			mockRsapiClient.SetupIRsapiClientBehavior();
			mockRsapiClient.SetupQueryBehavior();

			return mockRsapiClient;
		}

		private Mock<HttpMessageHandler> GetMockHttpMessageHandler()
		{
			Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>();

			_jsonBearer = _jsonBearer.Replace("@bearerToken", BearerToken);
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
