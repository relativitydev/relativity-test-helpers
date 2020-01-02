using Moq;
using NUnit.Framework;
using Relativity.Test.Helpers.Kepler;
using System.Threading.Tasks;

namespace Relativity.Tests.Helpers.Tests.Unit.Kepler
{
	[TestFixture]
	public class OAuth2HelperUnitTests
	{
		private Mock<IOAuth2Helper> Sut;

		[OneTimeSetUp]
		public void SetUp()
		{
			Sut = GetMockOAuth2Helper();
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
			string oAuth2Name = "OAuth2Client";

			// Act
			Services.Security.Models.OAuth2Client result = await Sut.Object.CreateOAuth2ClientAsync(username, oAuth2Name);

			// Assert
			Assert.IsTrue(result.Id.Equals("1"));
		}

		[Test]
		public void DeleteOAuth2ClientAsyncTest()
		{
			// Arrange
			string clientId = "1";

			// Act

			// Assert
			Assert.DoesNotThrow(() => Sut.Object.DeleteOAuth2ClientAsync(clientId).Wait());
		}

		[Test]
		public async Task GetBearerTokenAsyncTest()
		{
			// Arrange
			string protocol = "";
			string serverAddress = "";
			string clientId = "";
			string clientSecret = "";
			string scope = "";
			string grantType = "";

			// Act
			string result = await Sut.Object.GetBearerTokenAsync(protocol, serverAddress, clientId, clientSecret, scope, grantType);

			// Assert
			Assert.IsTrue(result.Equals("abc123"));
		}

		private Mock<IOAuth2Helper> GetMockOAuth2Helper()
		{
			Mock<IOAuth2Helper> mockOAuth2Helper = new Mock<IOAuth2Helper>();
			Services.Security.Models.OAuth2Client oAuth2Client = new Services.Security.Models.OAuth2Client
			{
				Id = "1",
				Name = "1"
			};
			string bearerToken = "abc123";

			mockOAuth2Helper
				.Setup(x => x.CreateOAuth2ClientAsync(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(Task.FromResult(oAuth2Client));

			mockOAuth2Helper
				.Setup(x => x.DeleteOAuth2ClientAsync(It.IsAny<string>()))
				.Returns(Task.CompletedTask);

			mockOAuth2Helper
				.Setup(x => x.GetBearerTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(Task.FromResult(bearerToken));

			return mockOAuth2Helper;
		}
	}
}
