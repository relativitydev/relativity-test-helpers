using DbContextHelper;
using Relativity.API;
using Relativity.Test.Helpers.ServiceFactory;
using System;
using System.Data.SqlClient;
using Relativity.Test.Helpers.Configuration.Models;
using Relativity.Test.Helpers.Database;
using Relativity.Test.Helpers.Logging;

namespace Relativity.Test.Helpers
{
	public class TestHelper : IHelper
	{
		public readonly ConfigurationModel Configs = null;

		public TestHelper(ConfigurationModel configs)
		{
			Configs = configs;
		}

		public IDBContext GetDBContext(int caseID)
		{
			//You can create a new DBcontext using kCura.Data.RowDataGeteway until Relativity versions lower than 9.6.85.9
			//kCura.Data.RowDataGateway.Context context = new kCura.Data.RowDataGateway.Context(SharedTestHelpers.ConfigurationHelper.SQL_SERVER_ADDRESS, string.Format("EDDS{0}", caseID == -1 ? "" : caseID.ToString()), SharedTestHelpers.ConfigurationHelper.SQL_USER_NAME, SharedTestHelpers.ConfigurationHelper.SQL_PASSWORD);
			//return new DBContext(context);

			//You can create a new DBcontext using DBContextHelper for Relativity versions equal to or greater than 9.6.85.9
			TestDbContext context;

				context = new TestDbContext(this.Configs.SQLServerAddress, $"EDDS{(caseID == -1 ? "" : caseID.ToString())}", this.Configs.SQLUserName, this.Configs.SQLPassword);


			return context;
		}

		public Guid GetGuid(int workspaceID, int artifactID)
		{
			var sql = "select ArtifactGuid from eddsdbo.ArtifactGuid where artifactId = @artifactId";
			var context = GetDBContext(workspaceID);
			var result = context.ExecuteSqlStatementAsScalar<Guid>(sql, new SqlParameter("artifactId", artifactID));
			return result;
		}

		public ISecretStore GetSecretStore()
		{
			throw new NotImplementedException();
		}

		public IInstanceSettingsBundle GetInstanceSettingBundle()
		{
			throw new NotImplementedException();
		}

		public ILogFactory GetLoggerFactory()
		{
			var consoleLogger = new ConsoleLogger();
			var factory = new TestLogFactory(consoleLogger);
			return factory;
		}

		public string GetSchemalessResourceDataBasePrepend(IDBContext context)
		{
			throw new NotImplementedException();
		}

		public IServicesMgr GetServicesManager()
		{
				return new ServicesManager(Configs);



		}

		public IUrlHelper GetUrlHelper()
		{
			return new URLHelper();
		}

		public string ResourceDBPrepend()
		{
			throw new NotImplementedException();
		}

		public string ResourceDBPrepend(IDBContext context)
		{
			throw new NotImplementedException();
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~TestHelper() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		public IStringSanitizer GetStringSanitizer(int workspaceID)
		{
			throw new NotImplementedException();
		}
		#endregion

	}


}
