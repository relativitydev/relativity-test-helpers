using Relativity.API;
using Relativity.API.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Relativity.Test.Helpers.Exceptions;

// ReSharper disable InconsistentNaming

namespace Relativity.Test.Helpers.Database
{
	internal class TestDbContext : IDBContext
	{
		private const string ConnectionString_PersistSecurityInfo = "False";
		private const string ConnectionString_PacketSize = "4096";
		private const string ConnectionString_ConnectTimeout = "30";
		private const int SqlCommand_DefaultTimeout = 30;
		private const string SqlExceptionMessage_Default = "An error occured when executing the SQL Statement. Check inner exception.";
		private const string SqlExceptionMessage_RollbackSuccess = "An error occured when executing the SQL Statement. Rollback Success. Check inner exception.";
		private const string SqlExceptionMessage_RollbackFail = "An error occured when executing the SQL Statement. Rollback Failed. Check inner exception.";
		private const string SqlExceptionMessage_ExecuteAsList = "An error occured when executing the SQL Statement as a List. Check inner exception.";
		private readonly SqlConnection _sqlConnection;
		private SqlTransaction _sqlTransaction;
		private SqlCommand _sqlCommand;
		private string ConnectionString { get; set; }
		public string Database { get; set; }
		public string ServerName { get; set; }
		public bool IsMasterDatabase => Database.Equals("EDDS", StringComparison.CurrentCultureIgnoreCase);

		public TestDbContext(string serverName, string database, string username, string password)
		{
			ServerName = serverName;
			Database = database;
			ConnectionString = $"data source={serverName};initial catalog={database};persist security info={ConnectionString_PersistSecurityInfo};user id={username};password={password};packet size={ConnectionString_PacketSize};connect timeout={ConnectionString_ConnectTimeout};";
			_sqlConnection = new SqlConnection(ConnectionString);
		}

		public DbParameter CreateDbParameter()
		{
			return new SqlParameter();
		}

		public SqlConnection GetConnection()
		{
			return GetConnection(true);
		}

		public SqlConnection GetConnection(bool openConnectionIfClosed)
		{
			if (openConnectionIfClosed)
			{
				if (_sqlConnection.State == ConnectionState.Broken)
				{
					_sqlConnection.Close();
				}

				if (_sqlConnection.State == ConnectionState.Closed)
				{
					_sqlConnection.Open();
				}
			}

			return _sqlConnection;
		}

		public void BeginTransaction()
		{
			_sqlConnection.Open();
			_sqlTransaction = _sqlConnection.BeginTransaction();
		}

		public SqlTransaction GetTransaction()
		{
			return _sqlTransaction;
		}

		public void CommitTransaction()
		{
			_sqlTransaction.Commit();
			_sqlTransaction = null;
			_sqlConnection.Close();
		}

		public void RollbackTransaction()
		{
			try
			{
				_sqlTransaction.Rollback();
				_sqlTransaction = null;
			}
			catch (Exception)
			{
				//Intentionally hide the exception
			}

			try
			{
				_sqlConnection.Close();
			}
			catch (Exception)
			{
				//Intentionally hide the exception
			}
		}

		public void RollbackTransaction(Exception originatingException)
		{
			//In the original implementation, this method would throw the originating exception if RollbackTransaction threw an exception. But now, RollbackTransaction has been changed to never throw an exception.Therefore, the originatingException is never used.
			RollbackTransaction();
		}

		public void ReleaseConnection()
		{
			if (_sqlTransaction == null)
			{
				_sqlConnection.Close();
			}

			ClearCurrentCommand();
		}

		private void ClearCurrentCommand()
		{
			if (_sqlCommand != null)
			{
				_sqlCommand.Parameters.Clear();
			}

			_sqlCommand = null;
		}

		public void Cancel()
		{
			if (_sqlCommand != null)
			{
				try
				{
					_sqlCommand.Cancel();
					_sqlCommand = null;
				}
				catch (Exception)
				{
					//Intentionally hide the exception
				}
			}
		}

		public DataTable ExecuteSqlStatementAsDataTable(string sqlStatement)
		{
			return ExecuteSqlStatementAsDataTable(sqlStatement, SqlCommand_DefaultTimeout, null);
		}

		public DataTable ExecuteSqlStatementAsDataTable(string sqlStatement, int timeoutValue)
		{
			return ExecuteSqlStatementAsDataTable(sqlStatement, timeoutValue, null);
		}

		public DataTable ExecuteSqlStatementAsDataTable(string sqlStatement, IEnumerable<SqlParameter> parameters)
		{
			return ExecuteSqlStatementAsDataTable(sqlStatement, SqlCommand_DefaultTimeout, parameters);
		}

		public DataTable ExecuteSqlStatementAsDataTable(string sqlStatement, int timeoutValue, IEnumerable<SqlParameter> parameters)
		{
			if (string.IsNullOrWhiteSpace(sqlStatement))
			{
				throw new ArgumentNullException(nameof(sqlStatement));
			}
			if (timeoutValue < 1)
			{
				throw new ArgumentException($"{nameof(timeoutValue)} is not valid. [Value: {timeoutValue}]. '{nameof(timeoutValue)}' should be greater than zero.");
			}

			DataTable returnDataTable = new DataTable();

			try
			{
				BeginTransaction();
				_sqlCommand = _sqlConnection.CreateCommand();
				_sqlCommand.Connection = _sqlConnection;
				_sqlCommand.Transaction = _sqlTransaction;
				_sqlCommand.CommandText = sqlStatement;
				_sqlCommand.CommandTimeout = timeoutValue;

				if (parameters != null)
				{
					List<SqlParameter> sqlParameters = parameters.ToList();
					if (sqlParameters.Any())
					{
						foreach (SqlParameter sqlParameter in sqlParameters)
						{
							_sqlCommand.Parameters.Add(sqlParameter);
						}
					}
				}

				using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(_sqlCommand))
				{
					//todo: log executing sql
					sqlDataAdapter.Fill(returnDataTable);
				}

				CommitTransaction();
			}
			catch (Exception sqlException)
			{
				//todo: log error

				try
				{
					RollbackTransaction();
					throw new DbContextHelperException(SqlExceptionMessage_RollbackSuccess, sqlException);
				}
				catch (Exception rollbackException)
				{
					throw new DbContextHelperException(SqlExceptionMessage_RollbackFail, rollbackException);
				}
			}
			finally
			{
				ReleaseConnection();
			}

			return returnDataTable;
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement)
		{
			return ExecuteSqlStatementAsScalar<T>(sqlStatement, null, SqlCommand_DefaultTimeout);
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement, IEnumerable<SqlParameter> parameters)
		{
			return ExecuteSqlStatementAsScalar<T>(sqlStatement, parameters, SqlCommand_DefaultTimeout);
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement, int timeoutValue)
		{
			return ExecuteSqlStatementAsScalar<T>(sqlStatement, null, timeoutValue);
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			if (string.IsNullOrWhiteSpace(sqlStatement))
			{
				throw new ArgumentNullException(nameof(sqlStatement));
			}
			if (timeoutValue < 1)
			{
				throw new ArgumentException($"{nameof(timeoutValue)} is not valid. [Value: {timeoutValue}]. '{nameof(timeoutValue)}' should be greater than zero.");
			}

			T returnObject;

			try
			{
				BeginTransaction();
				_sqlCommand = _sqlConnection.CreateCommand();
				_sqlCommand.Connection = _sqlConnection;
				_sqlCommand.Transaction = _sqlTransaction;
				_sqlCommand.CommandText = sqlStatement;
				_sqlCommand.CommandTimeout = timeoutValue;

				if (parameters != null)
				{
					List<SqlParameter> sqlParameters = parameters.ToList();
					if (sqlParameters.Any())
					{
						foreach (SqlParameter sqlParameter in sqlParameters)
						{
							_sqlCommand.Parameters.Add(sqlParameter);
						}
					}
				}

				//todo: log executing sql
				returnObject = (T)_sqlCommand.ExecuteScalar();

				CommitTransaction();
			}
			catch (Exception sqlException)
			{
				//todo: log error

				try
				{
					RollbackTransaction();
					throw new DbContextHelperException(SqlExceptionMessage_RollbackSuccess, sqlException);
				}
				catch (Exception rollbackException)
				{
					throw new DbContextHelperException(SqlExceptionMessage_RollbackFail, rollbackException);
				}
			}
			finally
			{
				ReleaseConnection();
			}

			return returnObject;
		}

		public T ExecuteSqlStatementAsScalar<T>(string sqlStatement, params SqlParameter[] parameters)
		{
			return ExecuteSqlStatementAsScalar<T>(sqlStatement, parameters, SqlCommand_DefaultTimeout);
		}

		public object ExecuteSqlStatementAsScalar(string sqlStatement, params SqlParameter[] parameters)
		{
			return ExecuteSqlStatementAsScalar(sqlStatement, parameters, SqlCommand_DefaultTimeout);
		}

		public object ExecuteSqlStatementAsScalar(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			if (string.IsNullOrWhiteSpace(sqlStatement))
			{
				throw new ArgumentNullException(nameof(sqlStatement));
			}
			if (timeoutValue < 1)
			{
				throw new ArgumentException($"{nameof(timeoutValue)} is not valid. [Value: {timeoutValue}]. '{nameof(timeoutValue)}' should be greater than zero.");
			}

			object returnObject;

			try
			{
				BeginTransaction();
				_sqlCommand = _sqlConnection.CreateCommand();
				_sqlCommand.Connection = _sqlConnection;
				_sqlCommand.Transaction = _sqlTransaction;
				_sqlCommand.CommandText = sqlStatement;
				_sqlCommand.CommandTimeout = timeoutValue;

				if (parameters != null)
				{
					List<SqlParameter> sqlParameters = parameters.ToList();
					if (sqlParameters.Any())
					{
						foreach (SqlParameter sqlParameter in sqlParameters)
						{
							_sqlCommand.Parameters.Add(sqlParameter);
						}
					}
				}

				//todo: log executing sql
				returnObject = _sqlCommand.ExecuteScalar();

				CommitTransaction();
			}
			catch (Exception sqlException)
			{
				//todo: log error

				try
				{
					RollbackTransaction();
					throw new DbContextHelperException(SqlExceptionMessage_RollbackSuccess, sqlException);
				}
				catch (Exception rollbackException)
				{
					throw new DbContextHelperException(SqlExceptionMessage_RollbackFail, rollbackException);
				}
			}
			finally
			{
				ReleaseConnection();
			}

			return returnObject;
		}

		public object ExecuteSqlStatementAsScalarWithInnerTransaction(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			return ExecuteSqlStatementAsScalar(sqlStatement, parameters, timeoutValue);
		}

		public int ExecuteNonQuerySQLStatement(string sqlStatement)
		{
			return ExecuteNonQuerySQLStatement(sqlStatement, null, SqlCommand_DefaultTimeout);
		}

		public int ExecuteNonQuerySQLStatement(string sqlStatement, int timeoutValue)
		{
			return ExecuteNonQuerySQLStatement(sqlStatement, null, timeoutValue);
		}

		public int ExecuteNonQuerySQLStatement(string sqlStatement, IEnumerable<SqlParameter> parameters)
		{
			return ExecuteNonQuerySQLStatement(sqlStatement, parameters, SqlCommand_DefaultTimeout);
		}

		public int ExecuteNonQuerySQLStatement(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			if (string.IsNullOrWhiteSpace(sqlStatement))
			{
				throw new ArgumentNullException(nameof(sqlStatement));
			}
			if (timeoutValue < 1)
			{
				throw new ArgumentException($"{nameof(timeoutValue)} is not valid. [Value: {timeoutValue}]. '{nameof(timeoutValue)}' should be greater than zero.");
			}

			int returnRowAffectedCount;

			try
			{
				BeginTransaction();
				_sqlCommand = _sqlConnection.CreateCommand();
				_sqlCommand.Connection = _sqlConnection;
				_sqlCommand.Transaction = _sqlTransaction;
				_sqlCommand.CommandText = sqlStatement;
				_sqlCommand.CommandTimeout = timeoutValue;

				if (parameters != null)
				{
					List<SqlParameter> sqlParameters = parameters.ToList();
					if (sqlParameters.Any())
					{
						foreach (SqlParameter sqlParameter in sqlParameters)
						{
							_sqlCommand.Parameters.Add(sqlParameter);
						}
					}
				}

				//todo: log executing sql
				returnRowAffectedCount = _sqlCommand.ExecuteNonQuery();

				CommitTransaction();
			}
			catch (Exception sqlException)
			{
				//todo: log error

				try
				{
					RollbackTransaction();
					throw new DbContextHelperException(SqlExceptionMessage_RollbackSuccess, sqlException);
				}
				catch (Exception rollbackException)
				{
					throw new DbContextHelperException(SqlExceptionMessage_RollbackFail, rollbackException);
				}
			}
			finally
			{
				ReleaseConnection();
			}

			return returnRowAffectedCount;
		}

		public DbDataReader ExecuteSqlStatementAsDbDataReader(string sqlStatement)
		{
			return ExecuteSqlStatementAsDbDataReader(sqlStatement, null, SqlCommand_DefaultTimeout);
		}

		public DbDataReader ExecuteSqlStatementAsDbDataReader(string sqlStatement, int timeoutValue)
		{
			return ExecuteSqlStatementAsDbDataReader(sqlStatement, null, timeoutValue);
		}

		public DbDataReader ExecuteSqlStatementAsDbDataReader(string sqlStatement, IEnumerable<DbParameter> parameters)
		{
			return ExecuteSqlStatementAsDbDataReader(sqlStatement, parameters, SqlCommand_DefaultTimeout);
		}

		public DbDataReader ExecuteSqlStatementAsDbDataReader(string sqlStatement, IEnumerable<DbParameter> parameters, int timeoutValue)
		{
			if (string.IsNullOrWhiteSpace(sqlStatement))
			{
				throw new ArgumentNullException(nameof(sqlStatement));
			}
			if (timeoutValue < 1)
			{
				throw new ArgumentException($"{nameof(timeoutValue)} is not valid. [Value: {timeoutValue}]. '{nameof(timeoutValue)}' should be greater than zero.");
			}

			SqlDataReader returnSqlDataReader;

			try
			{
				GetConnection(true);
				_sqlCommand = _sqlConnection.CreateCommand();
				_sqlCommand.Connection = _sqlConnection;
				_sqlCommand.CommandText = sqlStatement;
				_sqlCommand.CommandTimeout = timeoutValue;

				if (parameters != null)
				{
					List<DbParameter> sqlParameters = parameters.ToList();
					if (sqlParameters.Any())
					{
						foreach (DbParameter sqlParameter in sqlParameters)
						{
							_sqlCommand.Parameters.Add(sqlParameter);
						}
					}
				}

				//todo: log executing sql
				returnSqlDataReader = _sqlCommand.ExecuteReader();
			}
			catch (Exception sqlException)
			{
				//todo: log error
				throw new DbContextHelperException(SqlExceptionMessage_Default, sqlException);
			}
			finally
			{
				//todo: Releasing connection closes the reader as well. We need to implement a better way to release connection. Commenting this code for now until we have a resolution.
				//ReleaseConnection();
			}

			return returnSqlDataReader;
		}

		public DataTable ExecuteSQLStatementGetSecondDataTable(string sqlStatement, int timeout = -1)
		{
			if (string.IsNullOrWhiteSpace(sqlStatement))
			{
				throw new ArgumentNullException(nameof(sqlStatement));
			}
			if (timeout == -1)
			{
				timeout = SqlCommand_DefaultTimeout;
			}
			if (timeout < 1)
			{
				throw new ArgumentException($"{nameof(timeout)} is not valid. [Value: {timeout}. '{nameof(timeout)}' should be greater than zero.]");
			}

			DataTable returnDataTable;

			try
			{
				BeginTransaction();
				_sqlCommand = _sqlConnection.CreateCommand();
				_sqlCommand.Connection = _sqlConnection;
				_sqlCommand.Transaction = _sqlTransaction;
				_sqlCommand.CommandText = sqlStatement;
				_sqlCommand.CommandTimeout = timeout;

				using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(_sqlCommand))
				{
					//todo: log executing sql
					DataSet dataSet = new DataSet();
					sqlDataAdapter.Fill(dataSet);
					returnDataTable = dataSet.Tables[1];
				}

				CommitTransaction();
			}
			catch (Exception sqlException)
			{
				//todo: log error

				try
				{
					RollbackTransaction();
					throw new DbContextHelperException(SqlExceptionMessage_RollbackSuccess, sqlException);
				}
				catch (Exception rollbackException)
				{
					throw new DbContextHelperException(SqlExceptionMessage_RollbackFail, rollbackException);
				}
			}
			finally
			{
				ReleaseConnection();
			}

			return returnDataTable;
		}

		public SqlDataReader ExecuteSQLStatementAsReader(string sqlStatement, int timeout = -1)
		{
			if (timeout == -1)
			{
				timeout = SqlCommand_DefaultTimeout;
			}

			return (SqlDataReader)ExecuteSqlStatementAsDbDataReader(sqlStatement, null, timeout);
		}

		private CommandBehavior CalculateCommandBehaviorForReader(IDbCommand command, Boolean sequentialAccess)
		{
			CommandBehavior commandBehavior = CommandBehavior.CloseConnection;
			if (command.Transaction != null)
			{
				commandBehavior = CommandBehavior.Default;
			}

			if (sequentialAccess)
			{
				commandBehavior = (commandBehavior | CommandBehavior.SequentialAccess);
			}

			return commandBehavior;
		}

		public SqlDataReader ExecuteParameterizedSQLStatementAsReader(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue = -1, bool sequentialAccess = false)
		{
			if (string.IsNullOrWhiteSpace(sqlStatement))
			{
				throw new ArgumentNullException(nameof(sqlStatement));
			}
			if (timeoutValue == -1)
			{
				timeoutValue = SqlCommand_DefaultTimeout;
			}
			if (timeoutValue < 1)
			{
				throw new ArgumentException($"{nameof(timeoutValue)} is not valid. [Value: {timeoutValue}]. '{nameof(timeoutValue)}' should be greater than zero.");
			}

			SqlDataReader returnSqlDataReader;

			try
			{
				GetConnection(true);
				_sqlCommand = _sqlConnection.CreateCommand();
				_sqlCommand.Connection = _sqlConnection;
				_sqlCommand.CommandText = sqlStatement;
				_sqlCommand.CommandTimeout = timeoutValue;

				if (parameters != null)
				{
					List<SqlParameter> sqlParameters = parameters.ToList();
					if (sqlParameters.Any())
					{
						foreach (SqlParameter sqlParameter in sqlParameters)
						{
							_sqlCommand.Parameters.Add(sqlParameter);
						}
					}
				}

				//todo: log executing sql
				CommandBehavior commandBehavior = CalculateCommandBehaviorForReader(_sqlCommand, sequentialAccess);
				returnSqlDataReader = _sqlCommand.ExecuteReader(commandBehavior);
			}
			catch (Exception sqlException)
			{
				//todo: log error
				throw new DbContextHelperException(SqlExceptionMessage_Default, sqlException);
			}
			finally
			{
				//todo: Releasing connection closes the reader as well. We need to implement a better way to release connection. Commenting this code for now until we have a resolution.
				//ReleaseConnection();
			}

			return returnSqlDataReader;
		}

		public IEnumerable<T> ExecuteSQLStatementAsEnumerable<T>(string sqlStatement, Func<SqlDataReader, T> converter, int timeout = -1)
		{
			if (string.IsNullOrWhiteSpace(sqlStatement))
			{
				throw new ArgumentNullException(nameof(sqlStatement));
			}
			if (timeout == -1)
			{
				timeout = SqlCommand_DefaultTimeout;
			}
			if (timeout < 1)
			{
				throw new ArgumentException($"{nameof(timeout)} is not valid. [Value: {timeout}]. '{nameof(timeout)}' should be greater than zero.");
			}

			List<T> returnList = new List<T>();

			try
			{
				GetConnection(true);
				_sqlCommand = _sqlConnection.CreateCommand();
				_sqlCommand.Connection = _sqlConnection;
				_sqlCommand.CommandText = sqlStatement;
				_sqlCommand.CommandTimeout = timeout;

				//todo: log executing sql
				SqlDataReader sqlDataReader = _sqlCommand.ExecuteReader();

				while (sqlDataReader.Read())
				{
					returnList.Add(converter(sqlDataReader));
				}
			}
			catch (Exception sqlException)
			{
				//todo: log error
				throw new DbContextHelperException(SqlExceptionMessage_Default, sqlException);
			}
			finally
			{
				ReleaseConnection();
			}

			return returnList;
		}

		public DbDataReader ExecuteProcedureAsReader(string procedureName, IEnumerable<SqlParameter> parameters)
		{
			if (string.IsNullOrWhiteSpace(procedureName))
			{
				throw new ArgumentNullException(nameof(procedureName));
			}

			SqlDataReader returnSqlDataReader;

			try
			{
				GetConnection(true);
				_sqlCommand = _sqlConnection.CreateCommand();
				_sqlCommand.Connection = _sqlConnection;
				_sqlCommand.CommandText = procedureName;
				_sqlCommand.CommandTimeout = SqlCommand_DefaultTimeout;
				_sqlCommand.CommandType = CommandType.StoredProcedure;

				if (parameters != null)
				{
					List<SqlParameter> sqlParameters = parameters.ToList();
					if (sqlParameters.Any())
					{
						foreach (SqlParameter sqlParameter in sqlParameters)
						{
							_sqlCommand.Parameters.Add(sqlParameter);
						}
					}
				}

				//todo: log executing sql
				returnSqlDataReader = _sqlCommand.ExecuteReader();
			}
			catch (Exception sqlException)
			{
				//todo: log error
				throw new DbContextHelperException(SqlExceptionMessage_Default, sqlException);
			}
			finally
			{
				//todo: Releasing connection closes the reader as well. We need to implement a better way to release connection. Commenting this code for now until we have a resolution.
				//ReleaseConnection();
			}

			return returnSqlDataReader;
		}

		public int ExecuteProcedureNonQuery(string procedureName, IEnumerable<SqlParameter> parameters)
		{
			if (string.IsNullOrWhiteSpace(procedureName))
			{
				throw new ArgumentNullException(nameof(procedureName));
			}

			int returnRowAffectedCount;

			try
			{
				BeginTransaction();
				_sqlCommand = _sqlConnection.CreateCommand();
				_sqlCommand.Connection = _sqlConnection;
				_sqlCommand.Transaction = _sqlTransaction;
				_sqlCommand.CommandText = procedureName;
				_sqlCommand.CommandTimeout = SqlCommand_DefaultTimeout;
				_sqlCommand.CommandType = CommandType.StoredProcedure;

				if (parameters != null)
				{
					List<SqlParameter> sqlParameters = parameters.ToList();
					if (sqlParameters.Any())
					{
						foreach (SqlParameter sqlParameter in sqlParameters)
						{
							_sqlCommand.Parameters.Add(sqlParameter);
						}
					}
				}

				//todo: log executing sql
				returnRowAffectedCount = _sqlCommand.ExecuteNonQuery();

				CommitTransaction();
			}
			catch (Exception sqlException)
			{
				//todo: log error

				try
				{
					RollbackTransaction();
					throw new DbContextHelperException(SqlExceptionMessage_RollbackSuccess, sqlException);
				}
				catch (Exception rollbackException)
				{
					throw new DbContextHelperException(SqlExceptionMessage_RollbackFail, rollbackException);
				}
			}
			finally
			{
				ReleaseConnection();
			}

			return returnRowAffectedCount;
		}

		public DataSet ExecuteSqlStatementAsDataSet(string sqlStatement)
		{
			return ExecuteSqlStatementAsDataSet(sqlStatement, null, SqlCommand_DefaultTimeout);
		}

		public DataSet ExecuteSqlStatementAsDataSet(string sqlStatement, IEnumerable<SqlParameter> parameters)
		{
			return ExecuteSqlStatementAsDataSet(sqlStatement, parameters, SqlCommand_DefaultTimeout);
		}

		public DataSet ExecuteSqlStatementAsDataSet(string sqlStatement, int timeoutValue)
		{
			return ExecuteSqlStatementAsDataSet(sqlStatement, null, timeoutValue);
		}

		public DataSet ExecuteSqlStatementAsDataSet(string sqlStatement, IEnumerable<SqlParameter> parameters, int timeoutValue)
		{
			if (string.IsNullOrWhiteSpace(sqlStatement))
			{
				throw new ArgumentNullException(nameof(sqlStatement));
			}
			if (timeoutValue < 1)
			{
				throw new ArgumentException($"{nameof(timeoutValue)} is not valid. [Value: {timeoutValue}]. '{nameof(timeoutValue)}' should be greater than zero.");
			}

			DataSet returnDataSet = new DataSet();

			try
			{
				BeginTransaction();
				_sqlCommand = _sqlConnection.CreateCommand();
				_sqlCommand.Connection = _sqlConnection;
				_sqlCommand.Transaction = _sqlTransaction;
				_sqlCommand.CommandText = sqlStatement;
				_sqlCommand.CommandTimeout = SqlCommand_DefaultTimeout;

				if (parameters != null)
				{
					List<SqlParameter> sqlParameters = parameters.ToList();
					if (sqlParameters.Any())
					{
						foreach (SqlParameter sqlParameter in sqlParameters)
						{
							_sqlCommand.Parameters.Add(sqlParameter);
						}
					}
				}

				using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(_sqlCommand))
				{
					//todo: log executing sql
					sqlDataAdapter.Fill(returnDataSet);
				}

				CommitTransaction();
			}
			catch (Exception sqlException)
			{
				//todo: log error

				try
				{
					RollbackTransaction();
					throw new DbContextHelperException(SqlExceptionMessage_RollbackSuccess, sqlException);
				}
				catch (Exception rollbackException)
				{
					throw new DbContextHelperException(SqlExceptionMessage_RollbackFail, rollbackException);
				}
			}
			finally
			{
				ReleaseConnection();
			}

			return returnDataSet;
		}

		public void ExecuteSqlBulkCopy(IDataReader dataReader, ISqlBulkCopyParameters bulkCopyParameters)
		{
			throw new NotImplementedException();
		}

		/* Needed for Relativity DLLS that are 10.* and above
public void ExecuteSqlBulkCopy(IDataReader dataReader, ISqlBulkCopyParameters bulkCopyParameters)
{
		throw new NotImplementedException();
} */
	}
}