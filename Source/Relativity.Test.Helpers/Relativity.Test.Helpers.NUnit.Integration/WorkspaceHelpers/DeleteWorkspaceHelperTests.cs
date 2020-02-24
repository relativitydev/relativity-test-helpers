using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.Exceptions;
using Relativity.Test.Helpers.SharedTestHelpers;
using Relativity.Test.Helpers.WorkspaceHelpers;

namespace Relativity.Test.Helpers.NUnit.Integration.WorkspaceHelpers
{
	[TestFixture]
	public class DeleteWorkspaceHelperTests
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

			_workspaceId = CreateWorkspace.CreateWorkspaceAsync(_workspaceName,
				SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, _servicesManager,
				SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
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
			var success = DeleteWorkspace.DeleteTestWorkspace(_workspaceId, _servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			Assert.IsTrue(success);
		}

		[Test]
		public void DeleteWorkspaceTest_Failure()
		{
			var success = DeleteWorkspace.DeleteTestWorkspace(0, _servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			Assert.IsFalse(success);
		}
	}
}