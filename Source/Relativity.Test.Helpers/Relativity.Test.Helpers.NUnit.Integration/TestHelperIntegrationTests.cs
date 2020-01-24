using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using kCura.Relativity.Client;

namespace Relativity.Test.Helpers.NUnit.Integration
{
	public class TestHelperIntegrationTests
	{
		private IHelper SuT;

		[OneTimeSetUp]
		public void SetUp()
		{
			SuT = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			SuT = null;
		}

		//[Test]
		//public void GetDBContextTest()
		//{
		//	// Arrange
		//	IDBContext context;

		//	// Act
		//	context = SuT.GetDBContext(-1);
		//	context.GetConnection(true);
		//	context.ReleaseConnection();

		//	// Assert
		//	Assert.IsTrue(context.Database.Equals(TestConstants.Database.EddsDatabaseName, StringComparison.OrdinalIgnoreCase));
		//}

		[Test]
		public void GetLoggerFactoryTest()
		{
			// Arrange
			ILogFactory logFactory;

			// Act
			logFactory = SuT.GetLoggerFactory();
			logFactory.GetLogger().LogDebug("GetLoggerFactoryTest: Test Log");

			// Assert
			Assert.IsTrue(logFactory != null);
		}

		[Test]
		public void GetGuidTest()
		{
			// Arrange
			string _workspaceName = $"IntTest_{Guid.NewGuid()}";
			IServicesMgr servicesManager = SuT.GetServicesManager();
			int _workspaceId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName,
				SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, servicesManager,
				SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;

			// Act
			// Get the Guid of the workspace
			Guid guid = SuT.GetGuid(-1, _workspaceId);

			// Assert
			Assert.NotNull(guid);
			Assert.AreNotEqual(new Guid("00000000-0000-0000-0000-000000000000"), guid);

			//Delete Workspace
			WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceId, servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
		}
	}
}
