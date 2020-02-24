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
using Relativity.Test.Helpers.WorkspaceHelpers;

namespace Relativity.Test.Helpers.NUnit.Integration.ArtifactHelpers
{
	public class FoldersHelperIntegrationTests
	{
		private IHelper testHelper;
		private int _workspaceId;
		private IServicesMgr _servicesManager;
		private string _workspaceName;
		private IDBContext _dbContext;
		private KeplerHelper _keplerHelper;
		private bool useDbContext;

		[OneTimeSetUp]
		public void SetUp()
		{
			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}
			testHelper = new TestHelper(configDictionary);

			_workspaceName = $"IntTest_{Guid.NewGuid()}";
			_servicesManager = testHelper.GetServicesManager();
			_workspaceId = CreateWorkspace.CreateWorkspaceAsync(_workspaceName,
				SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, _servicesManager,
				SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;

			_keplerHelper = new KeplerHelper();
			bool isKeplerCompatible = _keplerHelper.IsVersionKeplerCompatibleAsync().Result;
			useDbContext = !isKeplerCompatible || ConfigurationHelper.FORCE_DBCONTEXT.Trim().ToLower().Equals("true");
			if (useDbContext)
			{
				_dbContext = testHelper.GetDBContext(_workspaceId);
			}
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			//Delete Workspace
			DeleteWorkspace.DeleteTestWorkspace(_workspaceId, _servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

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
			string folderName = "";
			if (useDbContext)
			{
				folderName = Folders.GetFolderName(rootFolderArtifactId, _dbContext);
			}
			else
			{
				folderName = Folders.GetFolderName(rootFolderArtifactId, _workspaceId, _keplerHelper);
			}

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
