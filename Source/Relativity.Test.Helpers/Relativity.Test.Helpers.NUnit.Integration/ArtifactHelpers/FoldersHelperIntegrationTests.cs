using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace Relativity.Test.Helpers.NUnit.Integration.ArtifactHelpers
{
	public class TestFolderIntegrationTests
	{
		private IHelper testHelper;
		private FoldersHelper SuT;
		private int _workspaceId;
		private IServicesMgr servicesManager;
		private string _workspaceName;

		[SetUp]
		public void SetUp()
		{
			testHelper = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			SuT = new FoldersHelper(new HttpRequestHelper());

			_workspaceName = $"IntTest_{Guid.NewGuid()}";
			IServicesMgr servicesManager = testHelper.GetServicesManager();
			_workspaceId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName,
				SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, servicesManager,
				SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
		}

		[TearDown]
		public void TearDown()
		{
			//Delete Workspace
			WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceId, servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			testHelper = null;
			SuT = null;
		}

		[Test]
		public void GetFolderName()
		{

			IServicesMgr servicesManager = null;
			int _workspaceId = 0;
			// Arrange

			int rootFolderArtifactId = FoldersHelper.GetRootFolderArtifactID(_workspaceId, servicesManager,
				ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			// Act
			string folderName = SuT.GetFolderName(rootFolderArtifactId, _workspaceId);

			// Assert
			Assert.AreEqual(_workspaceName, folderName);
		}
	}
}
