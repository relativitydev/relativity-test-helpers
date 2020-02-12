using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using kCura.Relativity.Client;
using TestHelpersKepler;
using TestHelpersKepler.Interfaces;
using TestHelpersKepler.Services;

namespace Relativity.Test.Helpers.NUnit.Integration
{
	public class TestHelperIntegrationTests
	{
		private IHelper SuT;
		private int _workspaceOneId;
		private int _workspaceTwoId;
		private IServicesMgr _servicesManager;
		private readonly string _workspaceName = $"IntTest_{Guid.NewGuid()}";
		private ILogFactory _logFactory;

		[OneTimeSetUp]
		public void SetUp()
		{
			//Arrange
			SuT = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			_servicesManager = SuT.GetServicesManager();

			_workspaceOneId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName,
				SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, _servicesManager,
				SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;

			_workspaceTwoId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName,
				SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, _servicesManager,
				SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			//Delete Workspaces
			WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceOneId, _servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceTwoId, _servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			_servicesManager = null;
			SuT = null;
		}

		[Test]
		public void GetLoggerFactoryTest()
		{
			// Act
			_logFactory = SuT.GetLoggerFactory();
			_logFactory.GetLogger().LogDebug("GetLoggerFactoryTest: Test Log");

			// Assert
			Assert.IsTrue(_logFactory != null);
		}

		[Test]
		public void GetGuidTest()
		{
			// Act
			// Get the Guid of the workspace
			Guid guidOne = SuT.GetGuid(-1, _workspaceOneId);
			Guid guidTwo = SuT.GetGuid(-1, _workspaceTwoId);

			// Assert
			Assert.NotNull(guidOne);
			Assert.AreNotEqual(new Guid("00000000-0000-0000-0000-000000000000"), guidOne);

			Assert.NotNull(guidTwo);
			Assert.AreNotEqual(new Guid("00000000-0000-0000-0000-000000000000"), guidTwo);

		}
	}
}
