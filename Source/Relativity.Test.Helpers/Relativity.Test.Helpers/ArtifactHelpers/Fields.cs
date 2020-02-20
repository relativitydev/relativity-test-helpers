﻿using kCura.Relativity.Client;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers.Request;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using kCura.Vendor.Castle.Core.Internal;
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
	public class Fields : IFieldsHelper
	{
		private static bool? _keplerCompatible;

		#region Public Methods

		public static int GetFieldArtifactID(String fieldname, IDBContext workspaceDbContext)
		{
			if (fieldname.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(fieldname), "Field name cannot be null or empty.");
			}

			var keplerHelper = new KeplerHelper();

			if (keplerHelper.ForceDbContext()) return GetFieldArtifactIDWithDbContext(fieldname, workspaceDbContext);

			if (_keplerCompatible == null)
			{
				_keplerCompatible = keplerHelper.IsVersionKeplerCompatibleAsync().Result;
			}

			if (!_keplerCompatible.Value) return GetFieldArtifactIDWithDbContext(fieldname, workspaceDbContext);

			var workspaceId = keplerHelper.GetWorkspaceIdFromDbContext(workspaceDbContext);
			return GetFieldArtifactID(fieldname, workspaceId, keplerHelper);
		}

		public static int GetFieldCount(IDBContext workspaceDbContext, int fieldArtifactId)
		{
			if (fieldArtifactId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(fieldArtifactId),"Invalid Field Artifact Id");
			}

			var keplerHelper = new KeplerHelper();

			if (keplerHelper.ForceDbContext()) return GetFieldCountWithDbContext(workspaceDbContext, fieldArtifactId);

			if (_keplerCompatible == null)
			{
				_keplerCompatible = keplerHelper.IsVersionKeplerCompatibleAsync().Result;
			}

			if (!_keplerCompatible.Value) return GetFieldCountWithDbContext(workspaceDbContext, fieldArtifactId);

			var workspaceId = keplerHelper.GetWorkspaceIdFromDbContext(workspaceDbContext);
			return GetFieldCount(fieldArtifactId, workspaceId, keplerHelper);
		}

		#endregion

		#region DbContext Methods

		private static int GetFieldArtifactIDWithDbContext(String fieldname, IDBContext workspaceDbContext)
		{
			string sqlquery = @"SELECT [ArtifactID] FROM [EDDSDBO].[Field] Where[DisplayName] like @fieldname";
			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@fieldname", SqlDbType.NVarChar) {Value = fieldname}
			};
			int artifactTypeId = workspaceDbContext.ExecuteSqlStatementAsScalar<int>(sqlquery, sqlParams);
			return artifactTypeId;
		}

		public static int GetFieldCountWithDbContext(IDBContext workspaceDbContext, int fieldArtifactId)
		{
			string sqlquery = String.Format(@"select count(*) from [EDDSDBO].[ExtendedField] where ArtifactID = @fieldArtifactId");
			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@fieldArtifactId", SqlDbType.NVarChar) {Value = fieldArtifactId}
			};
			int fieldCount = workspaceDbContext.ExecuteSqlStatementAsScalar<int>(sqlquery, sqlParams);
			return fieldCount;
		}

		#endregion

		#region Kepler Methods

		public static int GetFieldArtifactID(string fieldname, int workspaceId, KeplerHelper keplerHelper)
		{
			try
			{
				keplerHelper.UploadKeplerFiles();

				const string routeName = Constants.Kepler.RouteNames.GetFieldArtifactIdAsync;

				FieldArtifactIdBaseRequestModel requestModel = new FieldArtifactIdBaseRequestModel
				{
					FieldName = fieldname,
					WorkspaceId = workspaceId
				};

				var httpRequestHelper = new HttpRequestHelper();
				string responseString = httpRequestHelper.SendPostRequest(requestModel, routeName);
				FieldArtifactIdResponseModel responseModel = JsonConvert.DeserializeObject<FieldArtifactIdResponseModel>(responseString);

				return responseModel.ArtifactId;
			}
			catch (Exception exception)
			{
				throw new TestHelpersException($"Error Getting Field ArtifactID [{nameof(fieldname)}:{fieldname}]", exception);
			}
		}

		public static int GetFieldCount(int artifactId, int workspaceId, KeplerHelper keplerHelper)
		{
			try
			{
				keplerHelper.UploadKeplerFiles();

				const string routeName = Constants.Kepler.RouteNames.GetFieldCountAsync;

				FieldCountBaseRequestModel requestModel = new FieldCountBaseRequestModel
				{
					FieldArtifactId = artifactId,
					WorkspaceId = workspaceId
				};

				var httpRequestHelper = new HttpRequestHelper();
				string responseString = httpRequestHelper.SendPostRequest(requestModel, routeName);
				FieldCountResponseModel responseModel = JsonConvert.DeserializeObject<FieldCountResponseModel>(responseString);

				return responseModel.Count;
			}
			catch (Exception exception)
			{
				throw new TestHelpersException($"Error Getting Field Count [{nameof(artifactId)}:{artifactId}]", exception);
			}
		}

		#endregion

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