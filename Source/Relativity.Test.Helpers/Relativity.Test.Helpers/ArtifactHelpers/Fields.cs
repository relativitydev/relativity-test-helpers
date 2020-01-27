using kCura.Relativity.Client;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers.Request;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
using DTOs = kCura.Relativity.Client.DTOs;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models;

namespace Relativity.Test.Helpers.ArtifactHelpers
{
    /// <summary>
    /// 
    /// Helpers to interact with Fields in Relativity
    /// 
    /// </summary>
    /// 
    public class Fields
    {
		public static int GetFieldArtifactID(String fieldname, int workspaceId)
		{
			var routeName = "GetFieldArtifactId";

			var requestModel = new FieldArtifactIdRequestModel
			{
				FieldName = fieldname,
				WorkspaceId = workspaceId
			};

			var content = HttpRequestHelper<FieldArtifactIdRequestModel>.GetRequestContent(requestModel);
			var restAddress = HttpRequestHelper<FieldArtifactIdRequestModel>.GetRestAddress(routeName);

			FieldArtifactIdResponseModel responseModel;
			var client = HttpRequestHelper<FieldArtifactIdRequestModel>.GetClient();
			using (client)
			{
				var response = client.PostAsync(restAddress, content).Result;
				if (!response.IsSuccessStatusCode)
				{
					throw new Exception("Failed to get field Aritfact ID.");
				}
				var responseString = response.Content.ReadAsStringAsync().Result;
				responseModel = JsonConvert.DeserializeObject<FieldArtifactIdResponseModel>(responseString);
			}

			return responseModel.ArtifactId;
		}

		public static int GetFieldCount(int artifactId, int workspaceId)
		{
			var routeName = "GetFieldCount";

			var requestModel = new FieldCountRequestModel
			{
				FieldArtifactId = artifactId,
				WorkspaceId = workspaceId
			};

			var content = HttpRequestHelper<FieldCountRequestModel>.GetRequestContent(requestModel);
			var restAddress = HttpRequestHelper<FieldCountRequestModel>.GetRestAddress(routeName);

			FieldCountResponseModel responseModel;
			var client = HttpRequestHelper<FieldCountRequestModel>.GetClient();
			using (client)
			{
				var response = client.PostAsync(restAddress, content).Result;
				if (!response.IsSuccessStatusCode)
				{
					throw new Exception("Failed to get field count");
				}
				var responseString = response.Content.ReadAsStringAsync().Result;
				responseModel = JsonConvert.DeserializeObject<FieldCountResponseModel>(responseString);
			}

			return responseModel.Count;
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