using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Relativity.API;
using System.Data;

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
		public static Int32 GetFieldArtifactID(String fieldname, IDBContext workspaceDbContext)
		{
			string sqlquery = @"SELECT [ArtifactID] FROM [EDDSDBO].[Field] Where[DisplayName] like @fieldname";

			var sqlParams = new List<SqlParameter>
			{
				new SqlParameter("@fieldname", SqlDbType.NVarChar) {Value = fieldname}
			};

			Int32 artifactTypeId = workspaceDbContext.ExecuteSqlStatementAsScalar<Int32>(sqlquery, sqlParams);

			return artifactTypeId;
		}
	}
}
