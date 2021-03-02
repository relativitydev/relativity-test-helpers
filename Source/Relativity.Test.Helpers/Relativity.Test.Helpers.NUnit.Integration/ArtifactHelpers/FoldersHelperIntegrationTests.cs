using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.Exceptions;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Collections.Generic;

namespace Relativity.Test.Helpers.NUnit.Integration.ArtifactHelpers
{
	[TestFixture]
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
			_workspaceId = Helpers.WorkspaceHelpers.WorkspaceHelpers.CreateAsync(_servicesManager, _workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME).Result;

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
			Helpers.WorkspaceHelpers.WorkspaceHelpers.Delete(_servicesManager, _workspaceId);

			testHelper = null;
			_servicesManager = null;
			_dbContext = null;
		}

		[Test]
		public void GetFolderNameTest()
		{
			// Arrange
			int rootFolderArtifactId = FoldersHelper.GetRootFolderArtifactID(_workspaceId, _servicesManager,
				ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			// Act
			string folderName = "";
			if (useDbContext)
			{
				folderName = FoldersHelper.GetFolderName(rootFolderArtifactId, _dbContext);
			}
			else
			{
				folderName = FoldersHelper.GetFolderName(rootFolderArtifactId, _workspaceId, _keplerHelper);
			}

			// Assert
			Assert.AreEqual(_workspaceName, folderName);
		}

		[Test]
		public void CreateFolderTest()
		{
			var testFolderName = "test_folder";
			int folderArtifactId = FoldersHelper.CreateFolder(_servicesManager, _workspaceId, testFolderName);

			Assert.IsNotNull(folderArtifactId);
		}

		[Test]
		public void CreateFolderTest_Failure()
		{
			Assert.Throws<TestHelpersException>(() => FoldersHelper.CreateFolder(_servicesManager, _workspaceId, null));
		}
	}
}
