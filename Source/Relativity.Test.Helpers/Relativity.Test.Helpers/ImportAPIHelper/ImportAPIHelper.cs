using System;
using System.Data;
using System.IO;
using System.Reflection;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI;
using Relativity.API;
using Relativity.Test.Helpers.SqlHelpers;

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
		private const string control_Number = "Control Number";
		private const string Native_File = "Native File";


		public static void CreateDocumentswithFolderName(int WorkspaceArtifactId, int numberofDocuments, bool UploadNative, string folderName, IDBContext workspaceDbContext)
		{
			try
			{

			int identityFieldArtifactId = ArtifactHelpers.Fields.GetFieldArtifactID(Relativity.Test.Helpers.ArtifactHelpers.Document.GetDocumentIdentifierFieldName(workspaceDbContext, 10), workspaceDbContext);
			var iapi = new ImportAPI(SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD, string.Format("{0}://{1}/Relativitywebapi/", SharedTestHelpers.ConfigurationHelper.SERVER_BINDING_TYPE, SharedTestHelpers.ConfigurationHelper.RSAPI_SERVER_ADDRESS));
			var importJob = iapi.NewNativeDocumentImportJob();

			importJob.Settings.CaseArtifactId = WorkspaceArtifactId;
			importJob.Settings.ExtractedTextFieldContainsFilePath = false;

			// Indicates file path for the native file.
			importJob.Settings.NativeFilePathSourceFieldName = Native_File;

			// Indicates the column containing the ID of the parent document.
			importJob.Settings.ParentObjectIdSourceFieldName = PARENT_OBJECT_ID_SOURCE_FIELD_NAME;

			// The name of the document identifier column must match the name of the document identifier field
			// in the workspace.
			importJob.Settings.SelectedIdentifierFieldName = control_Number;
			importJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			importJob.Settings.OverwriteMode = OverwriteModeEnum.Append;

			// Specify the ArtifactID of the document identifier field, such as a control number.
			importJob.Settings.IdentityFieldId = identityFieldArtifactId;

			importJob.SourceData.SourceData = GetDocumentDataTable(numberofDocuments, UploadNative, folderName).CreateDataReader();

			importJob.OnComplete += report =>
			{
				Console.WriteLine("Native Import Finished");
				Console.WriteLine(report.FatalException);
			};

			importJob.OnComplete += report =>
			{
				Console.WriteLine("Native Import Finished");
				Console.WriteLine(report.FatalException);
			};

			importJob.OnFatalException += report =>
			{
				Console.WriteLine("Native Import Failed");
				Console.WriteLine(report.FatalException);
			};

			Console.WriteLine("Executing import...");

			importJob.Execute();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public static DataTable GetDocumentDataTable(int documentCount, bool UploadNative, string folderName)
		{
			var table = new DataTable();
			var nativeFilePath = String.Empty;

			table.Columns.Add(control_Number, typeof(string));
			table.Columns.Add(Native_File, typeof(string));
			table.Columns.Add(folderName, typeof(string));

			for (int i = 0; i < documentCount; i++)
			{
				String documentName = "IAPI"
					+ '-'
					+ (UploadNative ? "Native" : "Empty")
					+ '-'
					+ Guid.NewGuid();

				if (UploadNative)
				{
					//get image source to upload
					var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					var nativeName = @"\\\\FakeFilePath\Natives\SampleTextFile.txt";
					if (executableLocation != null)
					{
						nativeFilePath = Path.Combine(executableLocation, nativeName);
					}
				}
				else
				{
					nativeFilePath = @""; //no native upload
				}
				var folder = folderName;
				table.Rows.Add(documentName, nativeFilePath, folder);
			}

			return table;
		}

		public static void CreateDocumentsWithFolderArtifactID(Int32 workspaceID, int folderArtifactID, IDBContext workspaceDbContext, int documentCount = 5)
		{
			int identityFieldArtifactId = ArtifactHelpers.Fields.GetFieldArtifactID(Relativity.Test.Helpers.ArtifactHelpers.Document.GetDocumentIdentifierFieldName(workspaceDbContext, 10), workspaceDbContext);

			var iapi = new ImportAPI(SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD, string.Format("{0}://{1}/Relativitywebapi/", SharedTestHelpers.ConfigurationHelper.SERVER_BINDING_TYPE, SharedTestHelpers.ConfigurationHelper.RSAPI_SERVER_ADDRESS));
			var importJob = iapi.NewNativeDocumentImportJob();
		
			importJob.Settings.CaseArtifactId = workspaceID;
			importJob.Settings.ExtractedTextFieldContainsFilePath = false;

			// Indicates file path for the native file.
			importJob.Settings.NativeFilePathSourceFieldName = Native_File;

			// Indicates the column containing the ID of the parent document.
			importJob.Settings.ParentObjectIdSourceFieldName = PARENT_OBJECT_ID_SOURCE_FIELD_NAME;

			// The name of the document identifier column must match the name of the document identifier field
			// in the workspace.
			importJob.Settings.SelectedIdentifierFieldName = control_Number;
			importJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			importJob.Settings.OverwriteMode = OverwriteModeEnum.Append;

			// Specify the ArtifactID of the document identifier field, such as a control number.
			importJob.Settings.IdentityFieldId = identityFieldArtifactId;

			var documentDataTable = GetDocumentDataTable(folderArtifactID, workspaceID, documentCount, workspaceDbContext);
			importJob.SourceData.SourceData = documentDataTable.CreateDataReader();

			Console.WriteLine("Executing import...");

			importJob.Execute();

			// return artifact id's of documents
		}

		public static DataTable GetDocumentDataTable(int folderArtifactID, Int32 workspaceID, int documentCount, IDBContext workspaceDbContext)
		{
			var table = new DataTable();
			var FolderName =  Relativity.Test.Helpers.ArtifactHelpers.Folders.GetFolderName(folderArtifactID, workspaceDbContext);
			var nativeFilePath = String.Empty;

			table.Columns.Add(control_Number, typeof(string));
			table.Columns.Add(Native_File, typeof(string));
			table.Columns.Add(PARENT_OBJECT_ID_SOURCE_FIELD_NAME, typeof(string));

			for (int i = 0; i < documentCount; i++)
			{
				//get image source to upload
				var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				var nativeName = @"\\\\FakeFilePath\Natives\SampleTextFile.txt"; ;


				if (executableLocation != null)
				{
					nativeFilePath = Path.Combine(executableLocation, nativeName);
					var Control_NUM = "IAPI" + Guid.NewGuid();
					table.Rows.Add(Control_NUM, nativeFilePath, FolderName);
				}
			}

			return table;
		}

	}
}
