using kCura.Relativity.Client;
using Moq;
using NUnit.Framework;
using Relativity.Services.Security;
using Relativity.Test.Helpers.Kepler;
using Relativity.Tests.Helpers.Tests.Unit.MockHelpers;
using System.Net.Http;
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
			_jsonBearer = _jsonBearer.Replace("@bearerToken", BearerToken);
			_mockOAuth2ClientManager = MockOAuth2ClientManagerHelper.GetMockOAuth2ClientManager(OAuth2Id, OAuth2Name, OAuth2Secret, ContextUser);
			_mockRsapiClient = MockRsapiClientHelper.GetMockRsapiClientForUser();
			_mockHttpMessageHandler = MockHttpMessageHandlerHelper.GetMockHttpMessageHandlerForBearerToken(_jsonBearer);

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
	}
}
