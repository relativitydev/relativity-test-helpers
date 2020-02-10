using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace Relativity.Test.Helpers.NUnit.Integration.UserHelpers
{
	[TestFixture]
	public class UserIntegrationTests
	{
		private IHelper testHelper;
		private IServicesMgr servicesMgr;
		private IRSAPIClient rsapiClient;

		[OneTimeSetUp]
		public void SetUp()
		{
			testHelper = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			servicesMgr = testHelper.GetServicesManager();
			rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			testHelper = null;
			servicesMgr = null;
			rsapiClient = null;
		}

		[Test]
		public void CreateUserAndDeleteUserTest()
		{
			// Act
			int _userArtifactId = Relativity.Test.Helpers.UserHelpers.CreateUser.CreateNewUser(rsapiClient);
			bool deleteUserResult = Relativity.Test.Helpers.UserHelpers.DeleteUser.Delete_User(rsapiClient, _userArtifactId);

			// Assert
			Assert.IsTrue(_userArtifactId > 0);
			Assert.IsTrue(deleteUserResult);
		}
	}
}
