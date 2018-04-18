using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Relativity.Test.Helpers.ImportAPIHelper
{
	public static class ImportAPIHelper
	{
		/// <summary>
		/// 
		/// Import API Helpers although very powerful are version specific so if these helpers do not work for you, please check the DLL version of the Import API and any other DLL that the Import API is dependent on
		/// 
		/// </summary>

		private const string PARENT_OBJECT_ID_SOURCE_FIELD_NAME = "TestFolder"; // Add Field Name
		private const string Control_Number = "Control Number";
		private const string Native_File = "Native File";

		public static ImportBulkArtifactJob GetImportApi(int workspaceId)
		{
			var iapi = new ImportAPI(SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD, string.Format("{0}://{1}/Relativitywebapi/", SharedTestHelpers.ConfigurationHelper.SERVER_BINDING_TYPE, SharedTestHelpers.ConfigurationHelper.RSAPI_SERVER_ADDRESS));
			var fields = iapi.GetWorkspaceFields(workspaceId, 10);

			var identifier = fields.FirstOrDefault(x => x.FieldCategory == kCura.Relativity.ImportAPI.Enumeration.FieldCategoryEnum.Identifier);
			if (identifier == null)
			{
				throw new ApplicationException($"No identifier field found for workspace id {workspaceId}");
			}
			// job.Settings.IdentityFieldId = identifier.ArtifactID;

			var importJob = iapi.NewNativeDocumentImportJob();

			importJob.Settings.CaseArtifactId = workspaceId;
			importJob.Settings.ExtractedTextFieldContainsFilePath = false;

			// Indicates file path for the native file.
			importJob.Settings.NativeFilePathSourceFieldName = Native_File;

			// Indicates the column containing the ID of the parent document.
			importJob.Settings.ParentObjectIdSourceFieldName = PARENT_OBJECT_ID_SOURCE_FIELD_NAME;

			// The name of the document identifier column must match the name of the document identifier field
			// in the workspace.
			importJob.Settings.SelectedIdentifierFieldName = Control_Number;
			importJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			importJob.Settings.OverwriteMode = OverwriteModeEnum.Append;

			// Specify the ArtifactID of the document identifier field, such as a control number.
			importJob.Settings.IdentityFieldId = identifier.ArtifactID;


			return importJob;
		}

		public static void Import(ImportBulkArtifactJob importJob, DataTable data)
		{
			var errors = new List<Exception>();

			try
			{
				importJob.SourceData.SourceData = data.CreateDataReader();
				Console.WriteLine("Executing import...");

				importJob.OnError += report =>
				{
					foreach (var key in report.Keys)
					{
						var value = report[key];
						errors.Add(new Exception($"{key} : {value}"));
					}
				};

				importJob.OnFatalException += report =>
				{
					errors.Add(report.FatalException);
				};

				importJob.Execute();
				if (errors.Any())
				{
					throw new AggregateException(errors);
				}
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e);
				throw;
			}

		}

		private static void Import(int workspaceArtifactId, DataTable data)
		{
			var importJob = GetImportApi(workspaceArtifactId);
			Import(importJob, data);
		}

		public static void CreateDocumentswithFolderName(int workspaceArtifactId, int numberOfDocuments, string folderName, string nativePathName)
		{
			var hasNative = !string.IsNullOrEmpty(nativePathName);
			var dt = GetDocumentDataTable(numberOfDocuments, folderName, (i) => new ImportDocument
			{
				FilePath = nativePathName,
				Name = $"IAPI-{(hasNative ? "Native" : "Empty")}-{Guid.NewGuid()}"
			});
			Import(workspaceArtifactId, dt);
		}


		private static DataTable GetDocumentDataTable(int documentCount, string folderName, Func<int, ImportDocument> documentFileGenerator)
		{
			var table = new DataTable();

			table.Columns.Add(Control_Number, typeof(string));
			table.Columns.Add(Native_File, typeof(string));
			table.Columns.Add(PARENT_OBJECT_ID_SOURCE_FIELD_NAME, typeof(string));

			for (int i = 0; i < documentCount; i++)
			{
				var folder = folderName;
				var document = documentFileGenerator(i);
				table.Rows.Add(document.Name, document.FilePath, folder);
			}

			return table;
		}

	}
}
