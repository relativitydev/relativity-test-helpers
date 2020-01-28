using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace Relativity.Test.Helpers.NUnit.Integration
{
	public class TestDocumentIntegrationTests
	{
		private IHelper testHelper;
		private DocumentHelper SuT;

		[OneTimeSetUp]
		public void SetUp()
		{
			testHelper = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			SuT = new DocumentHelper(new HttpRequestHelper());
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			testHelper = null;
			SuT = null;
		}

		[Test]
		public void GetDocumentIdentifierFieldColumnName()
		{
			//// Arrange
			//string _workspaceName = $"IntTest_{Guid.NewGuid()}";
			//IServicesMgr servicesManager = testHelper.GetServicesManager();
			//int _workspaceId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName,
			//	SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, servicesManager,
			//	SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
			//int fieldArtifactTypeId;

			//// Act
			//// Get Folder Name
			//string folderName = SuT.GetDocumentIdentifierFieldColumnName(fieldArtifactId, _workspaceId);

			//// Assert
			//Assert.AreEqual(_workspaceName, folderName);

			////Delete Workspace
			//WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceId, servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
		}
	}
}
