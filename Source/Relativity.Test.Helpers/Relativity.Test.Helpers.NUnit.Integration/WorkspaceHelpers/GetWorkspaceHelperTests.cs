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
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using Relativity.Test.Helpers.SharedTestHelpers;
using Relativity.Test.Helpers.WorkspaceHelpers;

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
			_workspaceId = CreateWorkspace.CreateWorkspaceAsync(_workspaceName,
				SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, _servicesManager,
				SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
		}

		[OneTimeTearDown]
		public void Teardown()
		{
			DeleteWorkspace.DeleteTestWorkspace(_workspaceId, _servicesManager, ConfigurationHelper.ADMIN_USERNAME,
				ConfigurationHelper.DEFAULT_PASSWORD);
			_testHelper = null;
			_servicesManager = null;
		}

		[Test]
		public void GetWorkspaceTest()
		{
			var workspaceName = Helpers.WorkspaceHelpers.WorkspaceHelpers.GetWorkspaceName(_client, _workspaceId);

			Assert.AreEqual(_workspaceName, workspaceName);
		}

		[Test]
		public void GetWorkspaceTest_Failure()
		{
			Assert.Throws<APIException>(() => Helpers.WorkspaceHelpers.WorkspaceHelpers.GetWorkspaceName(_client, 0));
		}
	}
}