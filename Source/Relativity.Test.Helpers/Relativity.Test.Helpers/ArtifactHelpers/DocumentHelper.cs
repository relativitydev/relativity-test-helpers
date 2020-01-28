using Relativity.API;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Relativity.Test.Helpers.ArtifactHelpers.Interfaces;
using Relativity.Test.Helpers.Exceptions;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models;

namespace Relativity.Test.Helpers.ArtifactHelpers
{
	/// <summary>
	/// 
	/// Helpers to interact with Documents in Relativity
	/// 
	/// </summary>

	public class DocumentHelper : IDocumentHelper
	{
		private IHttpRequestHelper _httpRequestHelper;
		public DocumentHelper(IHttpRequestHelper httpRequestHelper)
		{
			_httpRequestHelper = httpRequestHelper;
		}

		public string GetDocumentIdentifierFieldColumnName(int fieldArtifactTypeID, int workspaceID)
		{
			try
			{
				const string routeName = "GetDocumentIdentifierFieldColumnName";

				GetDocumentIdentifierFieldColumnNameRequestModel requestModel = new GetDocumentIdentifierFieldColumnNameRequestModel
				{
					FieldArtifactTypeId = fieldArtifactTypeID,
					WorkspaceId = workspaceID
				};

				string responseString = _httpRequestHelper.SendPostRequest(requestModel, routeName);
				GetDocumentIdentifierFieldColumnNameResponseModel responseModel = JsonConvert.DeserializeObject<GetDocumentIdentifierFieldColumnNameResponseModel>(responseString);

				return responseModel.ColumnName;
			}
			catch (Exception exception)
			{
				throw new TestHelpersException($"Failed to Get Document Identifier Field Column Name [{nameof(fieldArtifactTypeID)}:{nameof(fieldArtifactTypeID)}]", exception);
			}
		}

		public string GetDocumentIdentifierFieldName(int fieldArtifactTypeID, int workspaceID)
		{
			try
			{
				const string routeName = "GetDocumentIdentifierFieldColumnName";

				GetDocumentIdentifierFieldNameRequestModel requestModel = new GetDocumentIdentifierFieldNameRequestModel
				{
					FieldArtifactTypeId = fieldArtifactTypeID,
					WorkspaceId = workspaceID
				};

				string responseString = _httpRequestHelper.SendPostRequest(requestModel, routeName);
				GetDocumentIdentifierFieldNameResponseModel responseModel = JsonConvert.DeserializeObject<GetDocumentIdentifierFieldNameResponseModel>(responseString);

				return responseModel.FieldName;
			}
			catch (Exception exception)
			{
				throw new TestHelpersException($"Failed to Get Document Identifier Field Name [{nameof(fieldArtifactTypeID)}:{nameof(fieldArtifactTypeID)}]", exception);
			}
		}
	}
}
