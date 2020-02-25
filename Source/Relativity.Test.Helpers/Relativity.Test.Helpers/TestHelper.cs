using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.Logging;
using Relativity.Test.Helpers.ServiceFactory;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using kCura.Relativity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.Exceptions;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models;
using TestHelpersKepler.Services;
using DbContextHelper;


namespace Relativity.Test.Helpers
{
	public class TestHelper : IHelper
	{
		private readonly string _username;
		private readonly string _password;
		private readonly AppConfigSettings _alternateConfig;
		private bool? _keplerCompatible;

		private readonly List<string> _keplerFileNames = new List<string>()
		{
			Constants.Kepler.SERVICES_DLL_NAME,
			Constants.Kepler.INTERFACES_DLL_NAME
		};

		public TestHelper(string username, string password)
		{
			_username = username;
			_password = password;
			_keplerCompatible = null;
		}

		public TestHelper(string configSectionName)
		{
			_alternateConfig = new AppConfigSettings(configSectionName);
			_username = _alternateConfig.AdminUserName;
			_password = _alternateConfig.AdminPassword;
			_keplerCompatible = null;
		}

		public TestHelper(Dictionary<string, string> configDictionary)
		{
			ConfigurationHelper.SetupConfiguration(configDictionary);
			_username = ConfigurationHelper.ADMIN_USERNAME;
			_password = ConfigurationHelper.DEFAULT_PASSWORD;
			_keplerCompatible = null;
		}

		[Obsolete("This constructor is deprecated. Use the TestHelper(Dictionary<string, string> configDictionary constructor instead.")]
		public TestHelper(TestContext testContext)
		{
			ConfigurationHelper.SetupConfiguration(testContext);
			_username = ConfigurationHelper.ADMIN_USERNAME;
			_password = ConfigurationHelper.DEFAULT_PASSWORD;
			_keplerCompatible = null;
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

		public Guid GetGuid(int workspaceID, int artifactID)
		{
			var keplerHelper = new KeplerHelper();

			if (keplerHelper.ForceDbContext()) return GetGuidFromDbContext(workspaceID, artifactID);
			
			if (_keplerCompatible == null)
			{
				_keplerCompatible = keplerHelper.IsVersionKeplerCompatibleAsync().Result;
			}

			if (!_keplerCompatible.Value) return GetGuidFromDbContext(workspaceID, artifactID);

			keplerHelper.UploadKeplerFiles();
			return GetGuidFromKeplerService(workspaceID, artifactID);

		}

		private Guid GetGuidFromDbContext(int workspaceId, int artifactId)
		{
			var sql = "select ArtifactGuid from eddsdbo.ArtifactGuid where artifactId = @artifactId";
			var context = GetDBContext(workspaceId);
			var result = context.ExecuteSqlStatementAsScalar<Guid>(sql, new SqlParameter("artifactId", artifactId));
			return result;
		}

		private Guid GetGuidFromKeplerService(int workspaceId, int artifactId)
		{
			const string routeName = Constants.Kepler.RouteNames.GetGuidAsync;

			var requestModel = new GetGuidRequestModel
			{
				ArtifactId = artifactId,
				WorkspaceId = workspaceId
			};

			IHttpRequestHelper httpRequestHelper = new HttpRequestHelper();
			var responseString = httpRequestHelper.SendPostRequest(requestModel, routeName);
			GetGuidResponseModel responseModel = JsonConvert.DeserializeObject<GetGuidResponseModel>(responseString);

			return responseModel.Guid;
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
