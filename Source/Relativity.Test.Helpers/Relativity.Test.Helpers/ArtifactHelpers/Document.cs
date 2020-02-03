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

	public class Document : IDocumentHelper
	{
		
		public static string GetDocumentIdentifierFieldColumnName(IDBContext workspaceDbContext, Int32 fieldArtifactTypeID)
		{
			#region DBContext Methods

			string sql = @"
            SELECT AVF.ColumnName FROM [EDDSDBO].[ExtendedField] EF WITH(NOLOCK)
            JOIN [EDDSDBO].[ArtifactViewField] AVF WITH(NOLOCK)
            ON EF.TextIdentifier = AVF.HeaderName
            WHERE EF.IsIdentifier = 1 AND EF.FieldArtifactTypeID = '@fieldArtifactTypeID'";

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@fieldArtifactTypeID", SqlDbType.NVarChar) {Value = fieldArtifactTypeID}
			};

			var columnName = workspaceDbContext.ExecuteSqlStatementAsScalar<String>(sql, sqlParams);
			return columnName;
		}
		public static string GetDocumentIdentifierFieldName(IDBContext workspaceDbContext, Int32 fieldArtifactTypeID)
		{
			const string sql = @"
            SELECT [TextIdentifier] FROM [EDDSDBO].[ExtendedField] WITH(NOLOCK)
            WHERE IsIdentifier = 1 AND FieldArtifactTypeID = @fieldArtifactTypeID";

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("fieldArtifactTypeID", SqlDbType.NVarChar) {Value = fieldArtifactTypeID}
			};

			var columnName = workspaceDbContext.ExecuteSqlStatementAsScalar<String>(sql, sqlParams);
			return columnName;
		}

		#endregion

		#region Kepler methods

		public static string GetDocumentIdentifierFieldColumnName(int fieldArtifactTypeID, int workspaceID)
		{
			try
			{
				const string routeName = "GetDocumentIdentifierFieldColumnNameAsync";

				GetDocumentIdentifierFieldColumnNameRequestModel requestModel = new GetDocumentIdentifierFieldColumnNameRequestModel
				{
					FieldArtifactTypeId = fieldArtifactTypeID,
					WorkspaceId = workspaceID
				};

				var httpRequestHelper = new HttpRequestHelper();
				string responseString = httpRequestHelper.SendPostRequest(requestModel, routeName);
				GetDocumentIdentifierFieldColumnNameResponseModel responseModel = JsonConvert.DeserializeObject<GetDocumentIdentifierFieldColumnNameResponseModel>(responseString);

				return responseModel.ColumnName;
			}
			catch (Exception exception)
			{
				throw new TestHelpersException($"Failed to Get Document Identifier Field Column Name [{nameof(fieldArtifactTypeID)}:{fieldArtifactTypeID}]", exception);
			}
		}

		public static string GetDocumentIdentifierFieldName(int fieldArtifactTypeID, int workspaceID)
		{
			try
			{
				const string routeName = "GetDocumentIdentifierFieldNameAsync";

				GetDocumentIdentifierFieldNameRequestModel requestModel = new GetDocumentIdentifierFieldNameRequestModel
				{
					FieldArtifactTypeId = fieldArtifactTypeID,
					WorkspaceId = workspaceID
				};

				var httpRequestHelper = new HttpRequestHelper();
				string responseString = httpRequestHelper.SendPostRequest(requestModel, routeName);
				GetDocumentIdentifierFieldNameResponseModel responseModel = JsonConvert.DeserializeObject<GetDocumentIdentifierFieldNameResponseModel>(responseString);

				return responseModel.FieldName;
			}
			catch (Exception exception)
			{
				throw new TestHelpersException($"Failed to Get Document Identifier Field Name [{nameof(fieldArtifactTypeID)}:{fieldArtifactTypeID}]", exception);
			}
		}

		#endregion
	}
}
