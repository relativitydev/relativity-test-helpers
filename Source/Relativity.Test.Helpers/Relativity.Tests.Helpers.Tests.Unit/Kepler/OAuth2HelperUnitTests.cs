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

		private const string Username = "test@test.com";
		private const string OAuth2Id = "OAuth2Id";
		private const string OAuth2Name = "OAuth2Name";
		private const string OAuth2Secret = "OAuth2Secret";
		private const int ContextUser = 999;
		private const string BearerToken = "abc1234567890.abc1234567890abc1234567890";

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

			// Act
			Services.Security.Models.OAuth2Client result = await Sut.CreateOAuth2ClientAsync(Username, OAuth2Name);

			// Assert
			Assert.IsTrue(result.Id.Equals(OAuth2Id));
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

			// Act
			Services.Security.Models.OAuth2Client result = await Sut.ReadOAuth2ClientAsync(OAuth2Name);

			// Assert
			Assert.IsTrue(result.Id.Equals(OAuth2Id));
		}

		[Test]
		public async Task DoesOAuth2ClientExistAsyncTest()
		{
			// Arrange

			// Act
			bool result = await Sut.DoesOAuth2ClientExistAsync(OAuth2Name);

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
				Id = OAuth2Id,
				Name = OAuth2Name,
				Secret = OAuth2Secret,
				ContextUser = ContextUser
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
