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
		private IDBContext _dbContext;
		private KeplerHelper _keplerHelper;
		private bool useDbContext;

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
			_workspaceId = Helpers.WorkspaceHelpers.WorkspaceHelpers.CreateAsync(_servicesManager, _workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME).Result;
			_keplerHelper = new KeplerHelper();

			bool isKeplerCompatible = _keplerHelper.IsVersionKeplerCompatibleAsync().Result;
			useDbContext = !isKeplerCompatible || ConfigurationHelper.FORCE_DBCONTEXT.Trim().ToLower().Equals("true");
			if (useDbContext)
			{
				_dbContext = testHelper.GetDBContext(_workspaceId);
			}
		}

		[TearDown]
		public void TearDown()
		{
			//Delete Workspace
			Helpers.WorkspaceHelpers.WorkspaceHelpers.Delete(_servicesManager, _workspaceId);

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
			string columnName = "";
			if (useDbContext)
			{
				columnName = Document.GetDocumentIdentifierFieldColumnName(_dbContext, fieldArtifactTypeId);
			}
			else
			{
				columnName = Document.GetDocumentIdentifierFieldColumnName(fieldArtifactTypeId, _workspaceId, _keplerHelper);
			}

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
			string fieldName = "";
			if (useDbContext)
			{
				fieldName = Document.GetDocumentIdentifierFieldName(_dbContext, fieldArtifactTypeId);
			}
			else
			{
				fieldName = Document.GetDocumentIdentifierFieldName(fieldArtifactTypeId, _workspaceId, _keplerHelper);
			}

			// Assert
			Assert.AreEqual(controlNumber, fieldName);
		}

		[Test]
		public void GetDocumentIdentifierFieldColumnName_InvalidFieldType()
		{
			// Arrange
			const int fieldArtifactTypeId = 0;

			// Act
			string columnName = "";
			if (useDbContext)
			{
				columnName = Document.GetDocumentIdentifierFieldColumnName(_dbContext, fieldArtifactTypeId);
			}
			else
			{
				columnName = Document.GetDocumentIdentifierFieldColumnName(fieldArtifactTypeId, _workspaceId, _keplerHelper);
			}

			// Assert
			Assert.AreEqual(null, columnName);
		}

		[Test]
		public void GetDocumentIdentifierFieldName_InvalidFieldType()
		{
			// Arrange
			const int fieldArtifactTypeId = 0;

			// Act
			string fieldName = "";
			if (useDbContext)
			{
				fieldName = Document.GetDocumentIdentifierFieldName(_dbContext, fieldArtifactTypeId);
			}
			else
			{
				fieldName = Document.GetDocumentIdentifierFieldName(fieldArtifactTypeId, _workspaceId, _keplerHelper);
			}

			// Assert
			Assert.AreEqual(null, fieldName);
		}
	}
}
