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
	public class FoldersHelperIntegrationTests
	{
		private IHelper testHelper;
		private int _workspaceId;
		private IServicesMgr _servicesManager;
		private string _workspaceName;
		private IDBContext _dbContext;

		[SetUp]
		public void SetUp()
		{
			testHelper = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			_workspaceName = $"IntTest_{Guid.NewGuid()}";
			_servicesManager = testHelper.GetServicesManager();
			_workspaceId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName,
				SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, _servicesManager,
				SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
			_dbContext = testHelper.GetDBContext(_workspaceId);

		}

		[TearDown]
		public void TearDown()
		{
			////Delete Workspace
			WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceId, _servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			testHelper = null;
			_servicesManager = null;
			_dbContext = null;
		}

		[Test]
		public void GetFolderName()
		{
			// Arrange
			int rootFolderArtifactId = Folders.GetRootFolderArtifactID(_workspaceId, _servicesManager,
				ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			// Act
			string folderName = Folders.GetFolderName(rootFolderArtifactId, _dbContext);

			// Assert
			Assert.AreEqual(_workspaceName, folderName);
		}
	}
}
