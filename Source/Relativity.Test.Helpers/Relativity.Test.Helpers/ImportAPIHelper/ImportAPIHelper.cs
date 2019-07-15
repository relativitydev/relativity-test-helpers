using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI;
using kCura.Relativity.ImportAPI.Enumeration;
using Relativity.Test.Helpers.ImportAPIHelper.Request;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Relativity.Test.Helpers.ImportAPIHelper
{
	/// <summary>
	/// 
	/// Import API Helpers although very powerful are version specific so if these helpers do not work for you, please check the DLL version of the Import API and any other DLL that the Import API is dependent on
	///
	/// WARNING: THIS CLASS DOES NOT SUPPORT CONNECTING TO MULTIPLE INSTANCES
	/// </summary>
	public static class ImportAPIHelper
	{
		#region Old constants

		const string PARENT_OBJECT_ID_SOURCE_FIELD_NAME = "Test Folder";
		const string CONTROL_NUMBER = "Control Number";

		#endregion

		private static readonly string IMPORT_API_ENDPOINT = $"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.RSAPI_SERVER_ADDRESS}/Relativitywebapi/";

		public static ImportBulkArtifactJob GetImportApi(int workspaceId)
		{
			var jobRequest = new ImportJobRequest(workspaceId)
			{
				ParentObjectIdSourceFieldName = PARENT_OBJECT_ID_SOURCE_FIELD_NAME
			};
			return GetImportJob(jobRequest);
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

			table.Columns.Add(CONTROL_NUMBER, typeof(string));
			table.Columns.Add(Constants.NATIVE_FILE, typeof(string));
			table.Columns.Add(PARENT_OBJECT_ID_SOURCE_FIELD_NAME, typeof(string));

			for (int i = 0; i < documentCount; i++)
			{
				var folder = folderName;
				var document = documentFileGenerator(i);
				table.Rows.Add(document.Name, document.FilePath, folder);
			}

			return table;
		}

		/// <summary>
		/// Get import job based on request
		/// </summary>
		/// <param name="request">request for import job</param>
		/// <returns></returns>
		public static ImportBulkArtifactJob GetImportJob(ImportJobRequest request, IImportAPI iapi = null)
		{
			iapi = iapi ?? new ImportAPI(
					ConfigurationHelper.ADMIN_USERNAME,
					ConfigurationHelper.DEFAULT_PASSWORD,
					IMPORT_API_ENDPOINT);

			var fields = iapi.GetWorkspaceFields(request.WorkspaceID, request.ArtifactTypeID);
			var identifier = fields.FirstOrDefault(field => field.FieldCategory == FieldCategoryEnum.Identifier);
			if (identifier == null)
			{
				throw new ApplicationException($"No identifier field found for workspace id {request.WorkspaceID}");
			}

			ImportBulkArtifactJob importJob;
			if (request.ArtifactTypeID == (int)ArtifactType.Document)
			{
				importJob = iapi.NewNativeDocumentImportJob();
			}
			else
			{
				importJob = iapi.NewObjectImportJob(request.ArtifactTypeID);
			}

			// The name of the document identifier column must match the name of the document identifier field
			// in the workspace.
			importJob.Settings.SelectedIdentifierFieldName = identifier.Name;
			// Specify the ArtifactID of the document identifier field, such as a control number.
			importJob.Settings.IdentityFieldId = identifier.ArtifactID;

			// Hydrate job properties based on request
			request.HydrateImportJob(importJob);

			return importJob;
		}
		/// <summary>
		/// Get data table from folder location
		/// </summary>
		/// <param name="importJob">import job related to dataTable</param>
		/// <param name="folder">native folder location to get files</param>
		/// <param name="prepareDataTableAction">action to customize datatable</param>
		/// <param name="fillDataRow">action to add extra values to a datarow</param>
		/// <returns></returns>
		public static DataTable GetDocumentDataTableFromFolder(
				this ImportBulkArtifactJob importJob,
				string folder,
				Action<DataTable> prepareDataTableAction = null,
				Action<DataRow> fillDataRow = null)
		{
			if (string.IsNullOrEmpty(folder))
			{
				throw new ArgumentNullException(nameof(folder));
			}
			if (!Directory.Exists(folder))
			{
				throw new DirectoryNotFoundException();
			}

			var dataTable = new DataTable();
			dataTable.Columns.Add(importJob.Settings.SelectedIdentifierFieldName);
			dataTable.Columns.Add(Constants.NATIVE_FILE);
			prepareDataTableAction?.Invoke(dataTable);

			var files = Directory.EnumerateFiles(folder);
			foreach (var file in files)
			{
				var dataRow = dataTable.NewRow();
				dataRow[importJob.Settings.SelectedIdentifierFieldName] = Path.GetFileName(file);
				dataRow[Constants.NATIVE_FILE] = Path.GetFullPath(file);
				fillDataRow?.Invoke(dataRow);
				dataTable.Rows.Add(dataRow);
			}

			return dataTable;
		}
		/// <summary>
		/// Wrapper to increase speed on creating files from a test data folder
		/// </summary>
		/// <param name="workspaceArtifactId">target workspace</param>
		/// <param name="folderName">location of test files</param>
		/// <param name="validateNativeTypes">use automatic engine to generate Relativity Native Type (defaulted False)</param>
		/// <param name="overwriteMode">how import will work on existant documents (defaulted APPEND)</param>
		public static void ImportDocumentsInFolder(int workspaceArtifactId, string folderName, bool validateNativeTypes = false, OverwriteModeEnum overwriteMode = OverwriteModeEnum.Append)
		{
			var request = new ImportJobRequest(workspaceArtifactId)
			{
				DisableNativeValidation = !validateNativeTypes,
				OverwriteMode = overwriteMode
			};
			var job = GetImportJob(request);
			var dataTable = job.GetDocumentDataTableFromFolder(folderName);
			Import(job, dataTable);
		}
	}
}
