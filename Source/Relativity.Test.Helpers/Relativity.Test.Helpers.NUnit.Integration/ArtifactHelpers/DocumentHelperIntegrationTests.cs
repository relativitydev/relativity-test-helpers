using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.SharedTestHelpers;
using Relativity.Test.Helpers.WorkspaceHelpers;

namespace Relativity.Test.Helpers.NUnit.Integration.ArtifactHelpers
{
	public class DocumentHelperIntegrationTests
	{
		private IHelper testHelper;
		private string _workspaceName;
		private IServicesMgr _servicesManager;
		private int _workspaceId;
		private IDBContext _dbContext;

		[SetUp]
		public void SetUp()
		{
			testHelper = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			_workspaceName = $"IntTest_{Guid.NewGuid()}";
			_servicesManager = testHelper.GetServicesManager();
			_workspaceId = CreateWorkspace.CreateWorkspaceAsync(_workspaceName,
				SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, _servicesManager,
				SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
			_dbContext = testHelper.GetDBContext(_workspaceId);
		}

		[TearDown]
		public void TearDown()
		{
			//Delete Workspace
			DeleteWorkspace.DeleteTestWorkspace(_workspaceId, _servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			testHelper = null;
			_dbContext = null;
			_servicesManager = null;
			_dbContext = null;
		}

		[Test]
		public void GetDocumentIdentifierFieldColumnName()
		{
			// Arrange
			const int fieldArtifactTypeId = 10;
			const string controlNumber = "ControlNumber";

			// Act
			string columnName = Document.GetDocumentIdentifierFieldColumnName(_dbContext, fieldArtifactTypeId);

			// Assert
			Assert.AreEqual(controlNumber, columnName);
		}

		[Test]
		public void GetDocumentIdentifierFieldName()
		{
			// Arrange
			const int fieldArtifactTypeId = 10;
			const string controlNumber = "Control Number";

			// Act
			string fieldName = Document.GetDocumentIdentifierFieldName(_dbContext, fieldArtifactTypeId);

			// Assert
			Assert.AreEqual(controlNumber, fieldName);
		}
	}
}
