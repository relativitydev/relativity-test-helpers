using kCura.Relativity.Client.DTOs;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

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
				workspaceId = Helpers.WorkspaceHelpers.WorkspaceHelpers.CreateAsync(Sut.GetServicesManager(), sampleWorkspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME).ConfigureAwait(false).GetAwaiter().GetResult();

				Relativity.Test.Helpers.ImportAPIHelper.ImportAPIHelper.CreateDocumentswithFolderName(workspaceId, numberOfDocumentsToCreate, folderName, nativeFilePath);

				Query<Document> query = new Query<Document>
				{
					Fields = new List<FieldValue> { new FieldValue("Control Number") }
				};

				using (IObjectManager objectManager = Sut.GetServicesManager().CreateProxy<IObjectManager>(ExecutionIdentity.System))
				{
					QueryRequest queryRequest = new QueryRequest()
					{
						ObjectType = new ObjectTypeRef { ArtifactTypeID = Constants.ArtifactTypeIds.Document },
						Fields = new List<FieldRef>()
						{
							new FieldRef { Name = "Name" }
						},
					};
					QueryResult result = objectManager.QueryAsync(workspaceId, queryRequest, 1, 1000).ConfigureAwait(false).GetAwaiter().GetResult();
					numberOfDocumentsCreated = result.Objects.Count;
				}

				Helpers.WorkspaceHelpers.WorkspaceHelpers.Delete(Sut.GetServicesManager(), workspaceId);
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
