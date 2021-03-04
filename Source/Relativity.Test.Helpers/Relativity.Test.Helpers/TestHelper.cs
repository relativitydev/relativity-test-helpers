using DbContextHelper;
using Relativity.API;
using Relativity.Services;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Test.Helpers.Exceptions;
using Relativity.Test.Helpers.Logging;
using Relativity.Test.Helpers.ServiceFactory;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Relativity.Test.Helpers
{
	public class TestHelper : IHelper
	{
		private readonly string _username;
		private readonly string _password;
		private readonly AppConfigSettings _alternateConfig;

		private readonly List<string> _keplerFileNames = new List<string>()
		{
			Constants.Kepler.SERVICES_DLL_NAME,
			Constants.Kepler.INTERFACES_DLL_NAME
		};

		public TestHelper(string username, string password)
		{
			_username = username;
			_password = password;
		}

		public TestHelper(string configSectionName)
		{
			_alternateConfig = new AppConfigSettings(configSectionName);
			_username = _alternateConfig.AdminUserName;
			_password = _alternateConfig.AdminPassword;
		}

		public TestHelper(Dictionary<string, string> configDictionary)
		{
			ConfigurationHelper.SetupConfiguration(configDictionary);
			_username = ConfigurationHelper.ADMIN_USERNAME;
			_password = ConfigurationHelper.DEFAULT_PASSWORD;
		}

		public static IHelper ForUser(string username, string password)
		{
			return new TestHelper(username, password);
		}

		public static IHelper System()
		{
			var username = SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME;
			var password = SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD;
			return ForUser(username, password);
		}

		public IDBContext GetDBContext(int caseID)
		{
			//You can create a new DBcontext using kCura.Data.RowDataGeteway until Relativity versions lower than 9.6.85.9
			//kCura.Data.RowDataGateway.Context context = new kCura.Data.RowDataGateway.Context(SharedTestHelpers.ConfigurationHelper.SQL_SERVER_ADDRESS, string.Format("EDDS{0}", caseID == -1 ? "" : caseID.ToString()), SharedTestHelpers.ConfigurationHelper.SQL_USER_NAME, SharedTestHelpers.ConfigurationHelper.SQL_PASSWORD);
			//return new DBContext(context);

			//You can create a new DBcontext using DBContextHelper for Relativity versions equal to or greater than 9.6.85.9
			DbContext context;

			if (_alternateConfig != null)
			{
				context = new DbContext(this._alternateConfig.SqlServerAddress, $"EDDS{(caseID == -1 ? "" : caseID.ToString())}", this._alternateConfig.SqlUserName, this._alternateConfig.SqlPassword);
			}
			else
			{
				context = new DbContext(SharedTestHelpers.ConfigurationHelper.SQL_SERVER_ADDRESS, $"EDDS{(caseID == -1 ? "" : caseID.ToString())}", SharedTestHelpers.ConfigurationHelper.SQL_USER_NAME, SharedTestHelpers.ConfigurationHelper.SQL_PASSWORD);
			}

			return context;
		}

		/// <summary>
		/// Gets the guid of any artifact of any object type in a workspace, but using this overloaded version will default to Workspace object type
		/// </summary>
		/// <param name="workspaceID"></param>
		/// <param name="artifactID"></param>
		/// <returns></returns>
		public Guid GetGuid(int workspaceID, int artifactID)
		{
			return GetGuid(workspaceID, artifactID, Constants.ArtifactTypeIds.Workspace);
		}

		/// <summary>
		/// Gets the guid of any artifact of any object type in a workspace
		/// </summary>
		/// <param name="workspaceID"></param>
		/// <param name="artifactID"></param>
		/// <param name="artifactTypeID"></param>
		/// <returns></returns>
		public Guid GetGuid(int workspaceID, int artifactID, int artifactTypeID)
		{
			try
			{
				using (IObjectManager objectManager = this.GetServicesManager().CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
				{
					QueryRequest queryRequest = new QueryRequest()
					{
						ObjectType = new ObjectTypeRef { ArtifactTypeID = artifactTypeID },
						Condition = new WholeNumberCondition("ArtifactId", NumericConditionEnum.EqualTo, artifactID).ToQueryString(),
						Fields = new List<FieldRef>()
						{
							new FieldRef { Name = "Guids" }
						},
					};
					QueryResult result = objectManager.QueryAsync(workspaceID, queryRequest, 1, 10).ConfigureAwait(false).GetAwaiter().GetResult();

					Guid workspaceGuid = result.Objects.First().Guids.First();
					return workspaceGuid;
				}
			}
			catch (Exception ex)
			{
				string errorMessage = $"Could not find Guid in {nameof(GetGuid)} in the workspace {nameof(workspaceID)}: {workspaceID} for {nameof(artifactID)}: {artifactID} - {ex.Message}";
				Console.WriteLine(errorMessage);
				throw new TestHelpersException(errorMessage);
			}
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
			return _alternateConfig != null ? new ServicesManager(_alternateConfig) : new ServicesManager(_username, _password);
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

		public void Dispose()
		{
		}

		public IStringSanitizer GetStringSanitizer(int workspaceID)
		{
			throw new NotImplementedException();
		}
	}
}
