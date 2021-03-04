using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Collections.Generic;

namespace Relativity.Test.Helpers.NUnit.Integration.ArtifactHelpers
{
	[TestFixture]
	public class DocumentHelperIntegrationTests
	{
		private IHelper testHelper;
		private string _workspaceName;
		private IServicesMgr _servicesManager;
		private int _workspaceId;

		[SetUp]
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
			_workspaceId = Helpers.WorkspaceHelpers.WorkspaceHelpers.CreateAsync(_servicesManager, _workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		[TearDown]
		public void TearDown()
		{
			//Delete Workspace
			Helpers.WorkspaceHelpers.WorkspaceHelpers.Delete(_servicesManager, _workspaceId);

			testHelper = null;
			_servicesManager = null;
		}


		[Test]
		public void GetDocumentIdentifierFieldName()
		{
			// Arrange
			const int fieldArtifactTypeId = 14;
			const string controlNumber = "Control Number";

			// Act
			string fieldName = "";
			fieldName = Document.GetDocumentIdentifierFieldName(_servicesManager, _workspaceId, fieldArtifactTypeId);

			// Assert
			Assert.AreEqual(controlNumber, fieldName);
		}

		[Test]
		public void GetDocumentIdentifierFieldName_InvalidFieldType()
		{
			// Arrange
			const int fieldArtifactTypeId = 0;

			// Act / Assert
			string fieldName = "";
			Assert.Throws<Exception>(() =>
				Document.GetDocumentIdentifierFieldName(_servicesManager, _workspaceId, fieldArtifactTypeId));
		}
	}
}
