using NUnit.Framework;
using Relativity.API;
using Relativity.Services.Interfaces.UserInfo;
using Relativity.Services.Objects;
using Relativity.Services.Security;
using Relativity.Test.Helpers.Kepler;
using Relativity.Test.Helpers.SharedTestHelpers;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.NUnit.Integration.Kepler
{
	[TestFixture]
	public class OAuth2HelperTests
	{
		private TestHelper _testHelper;
		private string oAuthName = "TestOAuth2";
		private OAuth2Helper Sut;

		[OneTimeSetUp]
		public void SetUp()
		{
			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}
			_testHelper = new TestHelper(configDictionary);

			IOAuth2ClientManager oAuth2ClientManager = _testHelper.GetServicesManager().CreateProxy<IOAuth2ClientManager>(ExecutionIdentity.System);
			IUserInfoManager userManager = _testHelper.GetServicesManager().CreateProxy<IUserInfoManager>(ExecutionIdentity.System);
			IObjectManager objectManager = _testHelper.GetServicesManager().CreateProxy<IObjectManager>(ExecutionIdentity.System);

			Sut = new OAuth2Helper(oAuth2ClientManager, userManager, objectManager);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			_testHelper = null;
			Sut = null;
		}

		[Test]
		public async Task CreateOAuth2ClientAsyncTest()
		{
			try
			{
				// Arrange
				await CleanupExistingOAuth();

				// Act
				Services.Security.Models.OAuth2Client oAuth2Client = await Sut.CreateOAuth2ClientAsync(ConfigurationHelper.ADMIN_USERNAME, oAuthName);

				// Assert
				Assert.IsTrue(oAuth2Client.Name.Equals(oAuthName));
			}
			finally
			{
				await CleanupExistingOAuth();
			}
		}

		[Test]
		public async Task DeleteOAuth2ClientAsyncTest()
		{
			try
			{
				// Arrange
				await CleanupExistingOAuth();
				Services.Security.Models.OAuth2Client oAuth2Client = await Sut.CreateOAuth2ClientAsync(ConfigurationHelper.ADMIN_USERNAME, oAuthName);

				// Act
				await Sut.DeleteOAuth2ClientAsync(oAuth2Client.Id);
				bool result = await Sut.DoesOAuth2ClientExistAsync(oAuthName);

				// Assert
				Assert.IsFalse(result);
			}
			finally
			{
				await CleanupExistingOAuth();
			}
		}

		[Test]
		public async Task ReadOAuth2ClientAsyncTest_Valid()
		{
			try
			{
				// Arrange
				await CleanupExistingOAuth();

				// Act
				Services.Security.Models.OAuth2Client oAuth2Client = await Sut.CreateOAuth2ClientAsync(ConfigurationHelper.ADMIN_USERNAME, oAuthName);
				Services.Security.Models.OAuth2Client result = await Sut.ReadOAuth2ClientAsync(oAuthName);

				// Assert
				Assert.IsTrue(oAuth2Client.Name.Equals(result.Name));
			}
			finally
			{
				await CleanupExistingOAuth();
			}
		}

		[Test]
		public async Task DoesOAuth2ClientExistAsyncTest_Valid()
		{
			try
			{
				// Arrange
				await CleanupExistingOAuth();

				// Act
				Services.Security.Models.OAuth2Client oAuth2Client = await Sut.CreateOAuth2ClientAsync(ConfigurationHelper.ADMIN_USERNAME, oAuthName);
				bool result = await Sut.DoesOAuth2ClientExistAsync(oAuthName);

				// Assert
				Assert.IsTrue(result);
			}
			finally
			{
				await CleanupExistingOAuth();
			}
		}

		[Test]
		public async Task DoesOAuth2ClientExistAsyncTest_Invalid()
		{
			// Arrange
			await CleanupExistingOAuth();

			// Act
			bool result = await Sut.DoesOAuth2ClientExistAsync(oAuthName);

			// Assert
			Assert.IsFalse(result);
		}



		[Test]
		public async Task GetBearerTokenAsyncTest()
		{
			try
			{
				// Arrange
				await CleanupExistingOAuth();
				Services.Security.Models.OAuth2Client oAuth2Client = await Sut.CreateOAuth2ClientAsync(ConfigurationHelper.ADMIN_USERNAME, oAuthName);

				string url = $"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.RELATIVITY_INSTANCE_ADDRESS}//Relativity.REST/api/Relativity.Services.User.IUserModule/User Manager/GetKeplerStatusAsync";

				// Act
				string bearerToken = await Sut.GetBearerTokenAsync(ConfigurationHelper.SERVER_BINDING_TYPE, ConfigurationHelper.RELATIVITY_INSTANCE_ADDRESS, oAuth2Client.Id, oAuth2Client.Secret);

				using (HttpClient httpClient = new HttpClient())
				{
					httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
					httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", " ");

					HttpResponseMessage result = await httpClient.GetAsync(url);

					// Assert
					Assert.IsTrue(bearerToken.Length > 10);
					Assert.IsTrue(result.StatusCode == HttpStatusCode.OK);
				}
			}
			finally
			{
				await CleanupExistingOAuth();
			}
		}

		private async Task CleanupExistingOAuth()
		{
			if (await Sut.DoesOAuth2ClientExistAsync(oAuthName))
			{
				Services.Security.Models.OAuth2Client oAuth2Client = await Sut.ReadOAuth2ClientAsync(oAuthName);
				await Sut.DeleteOAuth2ClientAsync(oAuth2Client.Id);
			}
		}
	}
}
