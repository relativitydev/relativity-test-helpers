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

namespace Relativity.Test.Helpers.NUnit.Integration
{
	public class TestImportApiHelper
	{
		private TestHelper Sut;

		[OneTimeSetUp]
		public void Setup()
		{
			Sut = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
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
			int workspaceId = ConfigurationHelper.WORKSPACEID;
			int numberOfDocumentsToCreate = 5;
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
			bool success = true;
			try
			{
				Relativity.Test.Helpers.ImportAPIHelper.ImportAPIHelper.CreateDocumentswithFolderName(workspaceId, numberOfDocumentsToCreate, folderName, nativeFilePath);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred Creating Documents", ex);
			}

			try
			{
				Query<Document> query = new Query<Document>
				{
					Fields = new List<FieldValue> { new FieldValue("Control Number") }
				};

				using (IRSAPIClient rsapiClient = Sut.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				{
					rsapiClient.APIOptions.WorkspaceID = workspaceId;
					QueryResultSet<Document> result = rsapiClient.Repositories.Document.Query(query, 0);
					numberOfDocumentsCreated = result.TotalCount;
					foreach (Result<Document> document in result.Results)
					{
						int artifactId = document.Artifact.ArtifactID;
						WriteResultSet<Document> deleteResultSet = rsapiClient.Repositories.Document.Delete(artifactId);
						if (!deleteResultSet.Success)
						{
							throw new Exception("An error occurred Deleting Documents");
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred Querying for Documents", ex);
			}

			// Assert
			Assert.AreEqual(numberOfDocumentsToCreate, numberOfDocumentsCreated);
		}
	}
}
