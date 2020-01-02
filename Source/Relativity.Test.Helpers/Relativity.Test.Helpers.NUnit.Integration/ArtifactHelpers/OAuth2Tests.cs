using kCura.Relativity.Client;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.Security;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.SharedTestHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.NUnit.Integration.ArtifactHelpers
{
	[TestFixture]
	public class OAuth2Tests
	{
		private TestHelper _testHelper;
		private string oAuthName = "TestOAuth2";
		[OneTimeSetUp]
		public void SetUp()
		{
			_testHelper = new TestHelper(TestContext.CurrentContext);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			_testHelper = null;
		}

		[Test]
		public async Task CreateOAuth2ClientAsyncTest()
		{
			// Arrange
			IOAuth2ClientManager oAuth2ClientManager = _testHelper.GetServicesManager().CreateProxy<IOAuth2ClientManager>(ExecutionIdentity.System);
			IRSAPIClient rsapiClient = _testHelper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System);

			// Act
			Services.Security.Models.OAuth2Client oAuth2Client = await OAuth2.CreateOAuth2ClientAsync(oAuth2ClientManager, rsapiClient, ConfigurationHelper.ADMIN_USERNAME, oAuthName);

			// Assert
			Assert.IsTrue(oAuth2Client.Name.Equals(oAuthName));
			await OAuth2.DeleteOAuth2ClientAsync(oAuth2ClientManager, oAuth2Client.Id);
		}

		[Test]
		public async Task DeleteOAuth2ClientAsyncTest()
		{
			// Arrange
			IOAuth2ClientManager oAuth2ClientManager = _testHelper.GetServicesManager().CreateProxy<IOAuth2ClientManager>(ExecutionIdentity.System);
			IRSAPIClient rsapiClient = _testHelper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System);

			// Act
			Services.Security.Models.OAuth2Client oAuth2Client = await OAuth2.CreateOAuth2ClientAsync(oAuth2ClientManager, rsapiClient, ConfigurationHelper.ADMIN_USERNAME, oAuthName);
			await OAuth2.DeleteOAuth2ClientAsync(oAuth2ClientManager, oAuth2Client.Id);

			// Assert
			List<Services.Security.Models.OAuth2Client> allOAuth2s = await oAuth2ClientManager.ReadAllAsync();
			bool exists = allOAuth2s.Exists(x => x.Id.Equals(oAuth2Client.Id));

			Assert.IsTrue(oAuth2Client.Name.Equals(oAuthName));
		}

		[Test]
		public async Task GetBearerTokenAsyncTest()
		{
			// Arrange
			IOAuth2ClientManager oAuth2ClientManager = _testHelper.GetServicesManager().CreateProxy<IOAuth2ClientManager>(ExecutionIdentity.System);
			IRSAPIClient rsapiClient = _testHelper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System);

			// Act
			Services.Security.Models.OAuth2Client oAuth2Client = await OAuth2.CreateOAuth2ClientAsync(oAuth2ClientManager, rsapiClient, ConfigurationHelper.ADMIN_USERNAME, oAuthName);
			string bearerToken = await OAuth2.GetBearerTokenAsync(ConfigurationHelper.SERVER_BINDING_TYPE, ConfigurationHelper.RELATIVITY_INSTANCE_ADDRESS, oAuth2Client.Id, oAuth2Client.Secret);

			// Assert
			Assert.IsTrue(bearerToken.Length > 10);
			await OAuth2.DeleteOAuth2ClientAsync(oAuth2ClientManager, oAuth2Client.Id);
		}
	}
}
