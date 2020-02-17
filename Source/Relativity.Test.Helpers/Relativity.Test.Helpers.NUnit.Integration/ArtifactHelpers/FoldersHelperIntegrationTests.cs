using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.Exceptions;
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

		[OneTimeSetUp]
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

		[OneTimeTearDown]
		public void TearDown()
		{
			////Delete Workspace
			WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceId, _servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			testHelper = null;
			_servicesManager = null;
			_dbContext = null;
		}

		[Test]
		public void GetFolderNameTest()
		{
			// Arrange
			int rootFolderArtifactId = Folders.GetRootFolderArtifactID(_workspaceId, _servicesManager,
				ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			// Act
			string folderName = Folders.GetFolderName(rootFolderArtifactId, _dbContext);

			// Assert
			Assert.AreEqual(_workspaceName, folderName);
		}

		[Test]
		public void CreateFolderTest()
		{
			var testFolderName = "test_folder";
			int folderArtifactId = Folders.CreateFolder(_servicesManager, _workspaceId, testFolderName);

			Assert.IsNotNull(folderArtifactId);
		}

		[Test]
		public void CreateFolderTest_Failure()
		{
			Assert.Throws<TestHelpersException>(()=>Folders.CreateFolder(_servicesManager, _workspaceId, null));
		}
	}
}
