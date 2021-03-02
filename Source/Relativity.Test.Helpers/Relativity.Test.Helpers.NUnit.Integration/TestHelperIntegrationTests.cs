using NUnit.Framework;
using Relativity.API;
using System;
using System.Collections.Generic;

namespace Relativity.Test.Helpers.NUnit.Integration
{
	[TestFixture]
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
			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}
			SuT = new TestHelper(configDictionary);

			_servicesManager = SuT.GetServicesManager();
			_workspaceOneId = Helpers.WorkspaceHelpers.WorkspaceHelpers.CreateAsync(_servicesManager, _workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME).Result;
			_workspaceTwoId = Helpers.WorkspaceHelpers.WorkspaceHelpers.CreateAsync(_servicesManager, _workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME).Result;
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			//Delete Workspaces
			Helpers.WorkspaceHelpers.WorkspaceHelpers.Delete(_servicesManager, _workspaceOneId);
			Helpers.WorkspaceHelpers.WorkspaceHelpers.Delete(_servicesManager, _workspaceTwoId);

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
