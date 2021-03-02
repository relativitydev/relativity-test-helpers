using NUnit.Framework;
using Relativity.API;
using System;
using System.Collections.Generic;

namespace Relativity.Test.Helpers.NUnit.Integration.WorkspaceHelpers
{
	[TestFixture]
	public class CreateWorkspaceHelperTests
	{
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
		}

		[OneTimeTearDown]
		public void Teardown()
		{
			Helpers.WorkspaceHelpers.WorkspaceHelpers.Delete(_servicesManager, _workspaceId);
			_testHelper = null;
			_servicesManager = null;
		}

		[Test]
		public void CreateWorkspaceTest()
		{
			_workspaceId = Helpers.WorkspaceHelpers.WorkspaceHelpers.CreateAsync(_servicesManager, _workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME).Result;
			Assert.IsNotNull(_workspaceId);
		}

		[Test]
		public void CreateWorkspaceTest_Failure()
		{
			var badTemplateName = "00template00";
			Assert.Throws<AggregateException>(() =>
					Helpers.WorkspaceHelpers.WorkspaceHelpers.CreateAsync(_servicesManager, _workspaceName, badTemplateName).Wait()
			);
		}
	}
}
