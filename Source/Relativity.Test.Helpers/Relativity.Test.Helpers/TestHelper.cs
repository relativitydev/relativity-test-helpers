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
			throw new NotImplementedException("DbContext is not the recommended process to get sql access. Instead build a custom kepler service for any sql needs.");
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
