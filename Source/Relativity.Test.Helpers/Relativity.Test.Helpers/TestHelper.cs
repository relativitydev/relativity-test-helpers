using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.Logging;
using Relativity.Test.Helpers.ServiceFactory;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using kCura.Relativity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.Exceptions;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models;


namespace Relativity.Test.Helpers
{
	public class TestHelper : IHelper
	{
		private readonly string _username;
		private readonly string _password;
		private readonly AppConfigSettings _alternateConfig;

		private readonly string _defaultAppGuid = "3E86B18F-8B55-45C4-9A57-9E0CBD7BAF46";
		private readonly string _keplerFileLocation = "";
		private List<string> keplerFileNames = new List<string>()
		{
			"TestHelpersKepler.Services.dll",
			"TestHelpersKepler.Interfaces.dll"
		};
		//private readonly string _keplerServicesDllName = "TestHelpersKepler.Services.dll";
		//private readonly string _keplerInterfacesDllName = "TestHelpersKepler.Interfaces.dll";

		public TestHelper(string username, string password)
		{
			_username = username;
			_password = password;
			InstallKeplerResourceFiles(keplerFileNames);
		}

		public TestHelper(string configSectionName)
		{
			_alternateConfig = new AppConfigSettings(configSectionName);
			_username = _alternateConfig.AdminUserName;
			_password = _alternateConfig.AdminPassword;
			InstallKeplerResourceFiles(keplerFileNames);
		}

		public TestHelper(Dictionary<string, string> configDictionary)
		{
			ConfigurationHelper.SetupConfiguration(configDictionary);
			_username = ConfigurationHelper.ADMIN_USERNAME;
			_password = ConfigurationHelper.DEFAULT_PASSWORD;
			InstallKeplerResourceFiles(keplerFileNames);
		}

		public TestHelper(TestContext testContext)
		{
			ConfigurationHelper.SetupConfiguration(testContext);
			_username = ConfigurationHelper.ADMIN_USERNAME;
			_password = ConfigurationHelper.DEFAULT_PASSWORD;
			InstallKeplerResourceFiles(keplerFileNames);
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
			throw new NotImplementedException();
		}

		public Guid GetGuid(int workspaceID, int artifactID)
		{
			const string routeName = "GetGuid";

			var requestModel = new GetGuidRequestModel
			{
				artifactID = artifactID,
				workspaceID = workspaceID
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

		private void InstallKeplerResourceFiles(List<string> keplerFiles)
		{
			foreach (var keplerDllName in keplerFiles)
			{
				using (IRSAPIClient rsapiClient = GetServiceFactory().CreateProxy<IRSAPIClient>())
				{
					var rfRequest = new ResourceFileRequest
					{
						AppGuid = new Guid(_defaultAppGuid),
						FullFilePath = _keplerFileLocation,
						FileName = keplerDllName
					};
					try
					{
						rsapiClient.PushResourceFiles(rsapiClient.APIOptions, new List<ResourceFileRequest>() { rfRequest });
						Console.WriteLine($"{nameof(InstallKeplerResourceFiles)} - File ({keplerDllName}) - was uploaded successfully");
					}
					catch (Exception ex)
					{
						Console.WriteLine($"{nameof(InstallKeplerResourceFiles)} - Could not upload ({keplerDllName}) - Exception: {ex.Message}");
					}
				}
			}
		}

		private Services.ServiceProxy.ServiceFactory GetServiceFactory()
		{
			var relativityServicesUri = new Uri($"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.RSAPI_SERVER_ADDRESS}/Relativity.Services");
			var relativityRestUri = new Uri($"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.REST_SERVER_ADDRESS.ToLower().Replace("-services", "")}/Relativity.Rest/Api");

			Relativity.Services.ServiceProxy.UsernamePasswordCredentials usernamePasswordCredentials = new Relativity.Services.ServiceProxy.UsernamePasswordCredentials(
				username: ConfigurationHelper.ADMIN_USERNAME,
				password: ConfigurationHelper.DEFAULT_PASSWORD);

			ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(
				relativityServicesUri: relativityServicesUri,
				relativityRestUri: relativityRestUri,
				credentials: usernamePasswordCredentials);

			var serviceFactory = new Services.ServiceProxy.ServiceFactory(
				settings: serviceFactorySettings);

			return serviceFactory;
		}
	}
}
