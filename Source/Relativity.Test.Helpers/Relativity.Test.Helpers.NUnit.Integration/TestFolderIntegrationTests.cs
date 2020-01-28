using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace Relativity.Test.Helpers.NUnit.Integration
{
	public class TestFolderIntegrationTests
	{
		private IHelper SuT;

		[OneTimeSetUp]
		public void SetUp()
		{
			SuT = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			SuT = null;
		}

		[Test]
		public void GetFolderName()
		{
			// Arrange
			string _workspaceName = $"IntTest_{Guid.NewGuid()}";
			IServicesMgr servicesManager = SuT.GetServicesManager();
			int _workspaceId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName,
				SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, servicesManager,
				SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;

			// Act
			// Get Folder Name
			var folderName = Relativity.Test.Helpers.ArtifactHelpers.Folders.GetFolderName(_rootFolderArtifactID, _workspaceId);

			// Assert
			//Assert.NotNull(guid);
			//Assert.AreNotEqual(new Guid("00000000-0000-0000-0000-000000000000"), guid);

			//Delete Workspace
			WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceId, servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
		}
	}
}
