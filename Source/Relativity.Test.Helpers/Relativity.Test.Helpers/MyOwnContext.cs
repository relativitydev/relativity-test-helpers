using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.API;

namespace Relativity.Test.Helpers
{
	class MyOwnContext : IDBContext
	{
		public SqlConnection GetConnection()
		{
			throw new NotImplementedException();
		}

		public DbParameter CreateDbParameter()
		{
			throw new NotImplementedException();
		}

		public SqlConnection GetConnection(bool openConnectionIfClosed)
		{
			throw new NotImplementedException();
		}

		public SqlTransaction GetTransaction()
		{
			throw new NotImplementedException();
		}

		public void BeginTransaction()
		{
			throw new NotImplementedException();
		}

		public void CommitTransaction()
		{
			throw new NotImplementedException();
		}

		public void RollbackTransaction()
		{
			throw new NotImplementedException();
		}

		public void RollbackTransaction(Exception originatingException)
		{
			throw new NotImplementedException();
		}

		public void ReleaseConnection()
		{
			throw new NotImplementedException();
		}

		public void Cancel()
		{
			throw new NotImplementedException();
		}

		public DataTable ExecuteSqlStatementAsDataTable(string sqlStatement)
		{
			throw new NotImplementedException();
		}

		public DataTable ExecuteSqlStatementAsDataTable(string sqlStatement, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public DataTable ExecuteSqlStatementAsDataTable(string sqlStatement, IEnumerable<SqlParameter> parameters)
		{
			throw new NotImplementedException();
		}

		public DataTable ExecuteSqlStatementAsDataTable(string sqlStatement, int timeoutValue, IEnumerable<SqlParameter> parameters)
		{
			throw new NotImplementedException();
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement)
		{
			throw new NotImplementedException();
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement, IEnumerable<SqlParameter> parameters)
		{
			throw new NotImplementedException();
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement, params SqlParameter[] parameters)
		{
			throw new NotImplementedException();
		}

		public object ExecuteSqlStatementAsScalar(string sqlStatement, params SqlParameter[] parameters)
		{
			throw new NotImplementedException();
		}

		public object ExecuteSqlStatementAsScalar(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public object ExecuteSqlStatementAsScalarWithInnerTransaction(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public int ExecuteNonQuerySQLStatement(string sqlStatement)
		{
			throw new NotImplementedException();
		}

		public int ExecuteNonQuerySQLStatement(string sqlStatement, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public int ExecuteNonQuerySQLStatement(string sqlStatement, IEnumerable<SqlParameter> parameters)
		{
			throw new NotImplementedException();
		}

		public int ExecuteNonQuerySQLStatement(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public DbDataReader ExecuteSqlStatementAsDbDataReader(string sqlStatement)
		{
			throw new NotImplementedException();
		}

		public DbDataReader ExecuteSqlStatementAsDbDataReader(string sqlStatement, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public DbDataReader ExecuteSqlStatementAsDbDataReader(string sqlStatement, IEnumerable<DbParameter> parameters)
		{
			throw new NotImplementedException();
		}

		public DbDataReader ExecuteSqlStatementAsDbDataReader(string sqlStatement, IEnumerable<DbParameter> parameters, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public DataTable ExecuteSQLStatementGetSecondDataTable(string sqlStatement, int timeout = -1)
		{
			throw new NotImplementedException();
		}

		public SqlDataReader ExecuteSQLStatementAsReader(string sqlStatement, int timeout = -1)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> ExecuteSQLStatementAsEnumerable<T>(string sqlStatement, Func<SqlDataReader, T> converter, int timeout = -1)
		{
			throw new NotImplementedException();
		}

		public DbDataReader ExecuteProcedureAsReader(string procedureName, IEnumerable<SqlParameter> parameters)
		{
			throw new NotImplementedException();
		}

		public int ExecuteProcedureNonQuery(string procedureName, IEnumerable<SqlParameter> parameters)
		{
			throw new NotImplementedException();
		}

		public SqlDataReader ExecuteParameterizedSQLStatementAsReader(string sqlStatement, IEnumerable<SqlParameter> parameters,
			int timeoutValue = -1, bool sequentialAccess = false)
		{
			throw new NotImplementedException();
		}

		public DataSet ExecuteSqlStatementAsDataSet(string sqlStatement)
		{
			throw new NotImplementedException();
		}

		public DataSet ExecuteSqlStatementAsDataSet(string sqlStatement, IEnumerable<SqlParameter> parameters)
		{
			throw new NotImplementedException();
		}

		public DataSet ExecuteSqlStatementAsDataSet(string sqlStatement, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public DataSet ExecuteSqlStatementAsDataSet(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			throw new NotImplementedException();
		}

		public string Database { get; }
		public string ServerName { get; }
		public bool IsMasterDatabase { get; }
	}
}
