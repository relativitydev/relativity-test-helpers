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

namespace Relativity.Test.Helpers.Fields
{
    /// <summary>
    /// 
    /// Helpers to interact with Fields in Relativity
    /// 
    /// </summary>
    /// 
    public class FieldHelper
    {

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
		public static int GetFieldArtifactID(String fieldname, IDBContext workspaceDbContext)
        {
            string sqlquery = @"SELECT [ArtifactID] FROM [EDDSDBO].[Field] Where[DisplayName] like @fieldname";
            var sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@fieldname", SqlDbType.NVarChar) {Value = fieldname}
            };
            int artifactTypeId = workspaceDbContext.ExecuteSqlStatementAsScalar<int>(sqlquery, sqlParams);
            return artifactTypeId;
        }
        public static int GetFieldCount(IDBContext workspaceDbContext, int fieldArtifactId)
        {
            string sqlquery = String.Format(@"select count(*) from [EDDSDBO].[ExtendedField] where ArtifactID = @fieldArtifactId");
            var sqlParams = new List<SqlParameter>
            {
                new SqlParameter("@fieldArtifactId", SqlDbType.NVarChar) {Value = fieldArtifactId}
            };
            int fieldCount = workspaceDbContext.ExecuteSqlStatementAsScalar<int>(sqlquery, sqlParams);
            return fieldCount;
        }
        public static int CreateField(IRSAPIClient client, FieldRequest request)
        {
            int fieldID = 0;
            //Set the workspace ID
            client.APIOptions.WorkspaceID = request.WorkspaceID;
            //Create a Field DTO
            var fieldDTO = new DTOs.Field();
            //Set secondary fields
            request.HydrateFieldDTO(fieldDTO);
            //Create the field
            var resultSet = client.Repositories.Field.Create(fieldDTO);
            //Check for success
            if (resultSet.Success)
            {
                fieldID = resultSet.Results.FirstOrDefault().Artifact.ArtifactID;
            }
            else
            {
                var innEx = resultSet.Results.Any() ? new Exception(resultSet.Results.First().Message) : null;
                throw new Exception(resultSet.Message, innEx);
            }
            return fieldID;
        }
        public static int CreateField_Date(IRSAPIClient client, int workspaceID)
        {
            var fieldRequest = new FieldRequest(workspaceID, FieldType.Date);
            return CreateField(client, fieldRequest);
        }
        public static int CreateField_User(IRSAPIClient client, int workspaceID)
        {
            var fieldRequest = new FieldRequest(workspaceID, FieldType.User);
            return CreateField(client, fieldRequest);
        }
        public static int CreateField_FixedLengthText(IRSAPIClient client, int workspaceID)
        {
            var fieldRequest = new FieldRequest(workspaceID, FieldType.FixedLengthText);
            return CreateField(client, fieldRequest);
        }
        public static int CreateField_LongText(IRSAPIClient client, int workspaceID)
        {
            var fieldRequest = new FieldRequest(workspaceID, FieldType.LongText);
            return CreateField(client, fieldRequest);
        }
        public static int CreateField_WholeNumber(IRSAPIClient client, int workspaceID)
        {
            var fieldRequest = new FieldRequest(workspaceID, FieldType.WholeNumber);
            return CreateField(client, fieldRequest);
        }
        public static int CreateField_YesNO(IRSAPIClient client, int workspaceID)
        {
            var fieldRequest = new FieldRequest(workspaceID, FieldType.YesNo);
            return CreateField(client, fieldRequest);
        }
        public static int CreateField_SingleChoice(IRSAPIClient client, int workspaceID)
        {
            var fieldRequest = new FieldRequest(workspaceID, FieldType.SingleChoice);
            return CreateField(client, fieldRequest);
        }
        public static int CreateField_MultipleChoice(IRSAPIClient client, int workspaceID)
        {
            var fieldRequest = new FieldRequest(workspaceID, FieldType.MultipleChoice);
            return CreateField(client, fieldRequest);
        }
    }
}