using kCura.Relativity.Client;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers.Request;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;
using Relativity.Test.Helpers.ArtifactHelpers.Interfaces;
using Relativity.Test.Helpers.Exceptions;
using Renci.SshNet;
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
	public class FieldsHelper : IFieldsHelper
	{
		private readonly IHttpRequestHelper _httpRequestHelper;

		public FieldsHelper(IHttpRequestHelper httpRequestHelper)
		{
			_httpRequestHelper = httpRequestHelper;
		}

		public int GetFieldArtifactId(string fieldname, int workspaceId)
		{
			try
			{
				const string routeName = "GetFieldArtifactIdAsync";

				FieldArtifactIdBaseRequestModel requestModel = new FieldArtifactIdBaseRequestModel
				{
					FieldName = fieldname,
					WorkspaceId = workspaceId
				};

				string responseString = _httpRequestHelper.SendPostRequest(requestModel, routeName);
				FieldArtifactIdResponseModel responseModel = JsonConvert.DeserializeObject<FieldArtifactIdResponseModel>(responseString);

				return responseModel.ArtifactId;
			}
			catch (Exception exception)
			{
				throw new TestHelpersException($"Error Getting Field ArtifactID [{nameof(fieldname)}:{fieldname}]", exception);
			}
		}

		public int GetFieldCount(int artifactId, int workspaceId)
		{
			try
			{
				const string routeName = "GetFieldCountAsync";

				FieldCountBaseRequestModel requestModel = new FieldCountBaseRequestModel
				{
					FieldArtifactId = artifactId,
					WorkspaceId = workspaceId
				};

				string responseString = _httpRequestHelper.SendPostRequest(requestModel, routeName);
				FieldCountResponseModel responseModel = JsonConvert.DeserializeObject<FieldCountResponseModel>(responseString);

				return responseModel.Count;
			}
			catch (Exception exception)
			{
				throw new TestHelpersException($"Error Getting Field Count [{nameof(artifactId)}:{artifactId}]", exception);
			}
		}
		public static int CreateField(IRSAPIClient client, FieldRequest request)
		{
			try
			{
				int fieldID = 0;
				//Set the workspace ID
				client.APIOptions.WorkspaceID = request.WorkspaceID;
				//Create a Field DTO
				DTOs.Field fieldDTO = new DTOs.Field();
				//Set secondary fields
				request.HydrateFieldDTO(fieldDTO);
				//Create the field
				DTOs.WriteResultSet<DTOs.Field> resultSet = client.Repositories.Field.Create(fieldDTO);
				//Check for success
				if (resultSet.Success)
				{
					fieldID = resultSet.Results.FirstOrDefault().Artifact.ArtifactID;
				}
				else
				{
					TestHelpersException innEx = resultSet.Results.Any() ? new TestHelpersException(resultSet.Results.First().Message) : null;
					throw new TestHelpersException(resultSet.Message, innEx);
				}
				return fieldID;
			}
			catch (Exception exception)
			{
				throw new TestHelpersException($"Error Creating Field [{nameof(request)}:{request}]");
			}
		}
		public static int CreateField_Date(IRSAPIClient client, int workspaceID)
		{
			FieldRequest fieldRequest = new FieldRequest(workspaceID, FieldType.Date);
			return CreateField(client, fieldRequest);
		}
		public static int CreateField_User(IRSAPIClient client, int workspaceID)
		{
			FieldRequest fieldRequest = new FieldRequest(workspaceID, FieldType.User);
			return CreateField(client, fieldRequest);
		}
		public static int CreateField_FixedLengthText(IRSAPIClient client, int workspaceID)
		{
			FieldRequest fieldRequest = new FieldRequest(workspaceID, FieldType.FixedLengthText);
			return CreateField(client, fieldRequest);
		}
		public static int CreateField_LongText(IRSAPIClient client, int workspaceID)
		{
			FieldRequest fieldRequest = new FieldRequest(workspaceID, FieldType.LongText);
			return CreateField(client, fieldRequest);
		}
		public static int CreateField_WholeNumber(IRSAPIClient client, int workspaceID)
		{
			FieldRequest fieldRequest = new FieldRequest(workspaceID, FieldType.WholeNumber);
			return CreateField(client, fieldRequest);
		}
		public static int CreateField_YesNO(IRSAPIClient client, int workspaceID)
		{
			FieldRequest fieldRequest = new FieldRequest(workspaceID, FieldType.YesNo);
			return CreateField(client, fieldRequest);
		}
		public static int CreateField_SingleChoice(IRSAPIClient client, int workspaceID)
		{
			FieldRequest fieldRequest = new FieldRequest(workspaceID, FieldType.SingleChoice);
			return CreateField(client, fieldRequest);
		}
		public static int CreateField_MultipleChoice(IRSAPIClient client, int workspaceID)
		{
			FieldRequest fieldRequest = new FieldRequest(workspaceID, FieldType.MultipleChoice);
			return CreateField(client, fieldRequest);
		}
	}
}