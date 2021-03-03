using NUnit.Framework;
using Relativity.API;
using System;
using System.Collections.Generic;

namespace Relativity.Test.Helpers.NUnit.Integration.WorkspaceHelpers
{
	[TestFixture]
	public class DeleteWorkspaceHelperTests
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

			_workspaceId = Helpers.WorkspaceHelpers.WorkspaceHelpers.CreateAsync(_servicesManager, _workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		[OneTimeTearDown]
		public void Teardown()
		{
			_testHelper = null;
			_servicesManager = null;
		}

		[Test]
		public void DeleteWorkspaceTest()
		{
			var success = Helpers.WorkspaceHelpers.WorkspaceHelpers.Delete(_servicesManager, _workspaceId);

			Assert.IsTrue(success);
		}

		[Test]
		public void DeleteWorkspaceTest_Failure()
		{
			var success = Helpers.WorkspaceHelpers.WorkspaceHelpers.Delete(_servicesManager, 0);

			Assert.IsFalse(success);
		}
	}
}