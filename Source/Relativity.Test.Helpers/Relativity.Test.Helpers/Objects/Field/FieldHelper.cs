using kCura.EventHandler;
using kCura.Relativity.Client;
using Relativity.API;
using Relativity.Test.Helpers.Fields.Request;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DTOs = kCura.Relativity.Client.DTOs;

namespace Relativity.Test.Helpers.Field
{
	/// <summary>
	///
	/// Helpers to interact with Fields in Relativity
	///
	/// </summary>
	///
	public class FieldHelper
	{
		private TestHelper _helper;

		public FieldHelper(TestHelper helper)
		{
			_helper = helper;
		}

		public FieldCollection ConvertFieldValuesToFieldCollection(List<DTOs.FieldValue> fieldValues)
		{
			FieldCollection fields = new FieldCollection();

			foreach (var fieldValue in fieldValues)
			{
				kCura.EventHandler.FieldValue ehFieldValue = new kCura.EventHandler.FieldValue(fieldValue.Value);
				kCura.EventHandler.Field field = new kCura.EventHandler.Field(fieldValue.ArtifactID, fieldValue.Name, "Column Name?", (int)fieldValue.FieldType, null, (int)fieldValue.FieldCategory, false, true, ehFieldValue, fieldValue.Guids);
				fields.Add(field);
			}

			return fields;
		}

		public int GetFieldArtifactID(int workspaceID, String fieldname)
		{
			string sqlquery = @"SELECT [ArtifactID] FROM [EDDSDBO].[Field] Where[DisplayName] like @fieldname";
			var sqlParams = new List<SqlParameter>
						{
								new SqlParameter("@fieldname", SqlDbType.NVarChar) {Value = fieldname}
						};
			int artifactTypeId = _helper.GetDBContext(workspaceID).ExecuteSqlStatementAsScalar<int>(sqlquery, sqlParams);
			return artifactTypeId;
		}

		public int GetFieldCount(int workspaceID, int fieldArtifactId)
		{
			string sqlquery = String.Format(@"select count(*) from [EDDSDBO].[ExtendedField] where ArtifactID = @fieldArtifactId");
			var sqlParams = new List<SqlParameter>
						{
								new SqlParameter("@fieldArtifactId", SqlDbType.NVarChar) {Value = fieldArtifactId}
						};
			int fieldCount = _helper.GetDBContext(workspaceID).ExecuteSqlStatementAsScalar<int>(sqlquery, sqlParams);
			return fieldCount;
		}

		public int CreateField(int workspaceID, FieldRequest request)
		{
			int fieldID = 0;
			DTOs.WriteResultSet<DTOs.Field> results;

			//Create a Field DTO
			var fieldDTO = new DTOs.Field();
			//Set secondary fields
			request.HydrateFieldDTO(fieldDTO);
			//Create the field
			using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
			{
				client.APIOptions.WorkspaceID = workspaceID;
				results = client.Repositories.Field.Create(fieldDTO);
			}
			//Check for success
			if (results.Success)
			{
				fieldID = results.Results.FirstOrDefault().Artifact.ArtifactID;
			}
			else
			{
				var innEx = results.Results.Any() ? new Exception(results.Results.First().Message) : null;
				throw new Exception(results.Message, innEx);
			}
			return fieldID;
		}

		public int CreateField_Date(int workspaceID)
		{
			var fieldRequest = new FieldRequest(workspaceID, FieldType.Date);
			return CreateField(workspaceID, fieldRequest);
		}

		public int CreateField_User(int workspaceID)
		{
			var fieldRequest = new FieldRequest(workspaceID, FieldType.User);
			return CreateField(workspaceID, fieldRequest);
		}

		public int CreateField_FixedLengthText(int workspaceID)
		{
			var fieldRequest = new FieldRequest(workspaceID, FieldType.FixedLengthText);
			return CreateField(workspaceID, fieldRequest);
		}

		public int CreateField_LongText(int workspaceID)
		{
			var fieldRequest = new FieldRequest(workspaceID, FieldType.LongText);
			return CreateField(workspaceID, fieldRequest);
		}

		public int CreateField_WholeNumber(int workspaceID)
		{
			var fieldRequest = new FieldRequest(workspaceID, FieldType.WholeNumber);
			return CreateField(workspaceID, fieldRequest);
		}

		public int CreateField_YesNO(int workspaceID)
		{
			var fieldRequest = new FieldRequest(workspaceID, FieldType.YesNo);
			return CreateField(workspaceID, fieldRequest);
		}

		public int CreateField_SingleChoice(int workspaceID)
		{
			var fieldRequest = new FieldRequest(workspaceID, FieldType.SingleChoice);
			return CreateField(workspaceID, fieldRequest);
		}

		public int CreateField_MultipleChoice(int workspaceID)
		{
			var fieldRequest = new FieldRequest(workspaceID, FieldType.MultipleChoice);
			return CreateField(workspaceID, fieldRequest);
		}
	}
}