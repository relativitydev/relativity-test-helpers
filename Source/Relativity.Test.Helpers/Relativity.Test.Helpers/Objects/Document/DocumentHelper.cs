using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Relativity.Test.Helpers.Objects.Document
{
	/// <summary>
	///
	/// Helpers to interact with Documents in Relativity
	///
	/// </summary>

	public class DocumentHelper
	{
		private TestHelper _helper;

		public DocumentHelper(TestHelper helper)
		{
			_helper = helper;
		}

		public string GetDocumentIdentifierFieldColumnName(Int32 fieldArtifactTypeID)
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

			var columnName = _helper.GetDBContext(-1).ExecuteSqlStatementAsScalar<String>(sql, sqlParams);
			return columnName;
		}

		public string GetDocumentIdentifierFieldName(int workspaceID, Int32 fieldArtifactTypeID)
		{
			const string sql = @"
            SELECT [TextIdentifier] FROM [EDDSDBO].[ExtendedField] WITH(NOLOCK)
            WHERE IsIdentifier = 1 AND FieldArtifactTypeID = @fieldArtifactTypeID";

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("fieldArtifactTypeID", SqlDbType.NVarChar) {Value = fieldArtifactTypeID}
			};

			var columnName = _helper.GetDBContext(workspaceID).ExecuteSqlStatementAsScalar<String>(sql, sqlParams);
			return columnName;
		}
	}
}