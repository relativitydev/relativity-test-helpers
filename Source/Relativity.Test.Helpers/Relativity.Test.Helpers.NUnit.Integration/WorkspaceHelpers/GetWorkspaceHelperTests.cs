using kCura.Relativity.Client;
using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.Exceptions;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Collections.Generic;

namespace Relativity.Test.Helpers.NUnit.Integration.WorkspaceHelpers
{
	[TestFixture]
	public class GetWorkspaceHelperTests
	{
		private IRSAPIClient _client;
		private int _workspaceId;
		private string _workspaceName = $"IntTest_{Guid.NewGuid()}";
		private IServicesMgr _servicesManager;
		private IHelper _testHelper;


		[OneTimeSetUp]
		public void SetUp()
		{
			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}
			_testHelper = new TestHelper(configDictionary);
			_servicesManager = _testHelper.GetServicesManager();
			_client = _testHelper.GetServicesManager().GetProxy<IRSAPIClient>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			_workspaceId = Helpers.WorkspaceHelpers.WorkspaceHelpers.CreateAsync(_servicesManager, _workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME).Result;
		}

		[OneTimeTearDown]
		public void Teardown()
		{
			Helpers.WorkspaceHelpers.WorkspaceHelpers.Delete(_servicesManager, _workspaceId);
			_testHelper = null;
			_servicesManager = null;
		}

		[Test]
		public void GetWorkspaceTest()
		{
			var workspaceName = Helpers.WorkspaceHelpers.WorkspaceHelpers.GetWorkspaceName(_servicesManager, _workspaceId);

			Assert.AreEqual(_workspaceName, workspaceName);
		}

		[Test]
		public void GetWorkspaceTest_Failure()
		{
			Assert.Throws<TestHelpersException>(() => Helpers.WorkspaceHelpers.WorkspaceHelpers.GetWorkspaceName(_servicesManager, 0));
		}
	}
}