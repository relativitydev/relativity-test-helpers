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

namespace Relativity.Test.Helpers.NUnit.Integration
{
	public class TestFolderIntegrationTests
	{
		private IHelper testHelper;
		private FoldersHelper SuT;

		[OneTimeSetUp]
		public void SetUp()
		{
			testHelper = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			SuT = new FoldersHelper(new HttpRequestHelper());
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			testHelper = null;
			SuT = null;
		}

		[Test]
		public void GetFolderName()
		{
			// Arrange
			string _workspaceName = $"IntTest_{Guid.NewGuid()}";
			IServicesMgr servicesManager = testHelper.GetServicesManager();
			int _workspaceId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName,
				SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, servicesManager,
				SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
			int rootFolderArtifactId = FoldersHelper.GetRootFolderArtifactID(_workspaceId, servicesManager,
				ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			
			// Act
			// Get Folder Name
			string folderName = SuT.GetFolderName(rootFolderArtifactId, _workspaceId);

			// Assert
			Assert.AreEqual(_workspaceName, folderName);

			//Delete Workspace
			WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceId, servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
		}
	}
}
