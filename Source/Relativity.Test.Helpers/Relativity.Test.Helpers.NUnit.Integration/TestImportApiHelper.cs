using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.SharedTestHelpers;
using Relativity.Test.Helpers.WorkspaceHelpers;

namespace Relativity.Test.Helpers.NUnit.Integration
{
	[TestFixture]
	public class TestImportApiHelper
	{
		private TestHelper Sut;

		[OneTimeSetUp]
		public void Setup()
		{
			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}
			Sut = new TestHelper(configDictionary);
		}

		[OneTimeTearDown]
		public void Teardown()
		{
			Sut = null;
		}

		[Test]
		public void CreateDocumentsWithFolderName()
		{
			// Arrange
			int workspaceId;
			int numberOfDocumentsToCreate = 5;
			string sampleWorkspaceName = "Integration Test Workspace";
			string folderName = "Sample Folder";
			string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string nativeName = @"\\\\FakeFilePath\Natives\SampleTextFile.txt";
			string nativeFilePath = "";
			int numberOfDocumentsCreated = 0;
			if (executableLocation != null)
			{
				nativeFilePath = Path.Combine(executableLocation, nativeName);
			}

			// Act
			try
			{
				workspaceId = CreateWorkspace.CreateWorkspaceAsync(sampleWorkspaceName, ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, Sut.GetServicesManager(), ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD).Result;

				Relativity.Test.Helpers.ImportAPIHelper.ImportAPIHelper.CreateDocumentswithFolderName(workspaceId, numberOfDocumentsToCreate, folderName, nativeFilePath);

				Query<Document> query = new Query<Document>
				{
					Fields = new List<FieldValue> { new FieldValue("Control Number") }
				};

				using (IRSAPIClient rsapiClient = Sut.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				{
					rsapiClient.APIOptions.WorkspaceID = workspaceId;
					QueryResultSet<Document> result = rsapiClient.Repositories.Document.Query(query, 0);
					numberOfDocumentsCreated = result.TotalCount;
				}

				DeleteWorkspace.DeleteTestWorkspace(workspaceId, Sut.GetServicesManager(), ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred", ex);
			}

			// Assert
			Assert.AreEqual(numberOfDocumentsToCreate, numberOfDocumentsCreated);
		}
	}
}
