using Relativity.API;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Relativity.Test.Helpers.ArtifactHelpers
{
	/// <summary>
	/// 
	/// Helpers to interact with Documents in Relativity
	/// 
	/// </summary>

	public class Document
	{
		public static string GetDocumentIdentifierFieldColumnName(IDBContext workspaceDbContext, Int32 fieldArtifactTypeID)
		{

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

	}
}
