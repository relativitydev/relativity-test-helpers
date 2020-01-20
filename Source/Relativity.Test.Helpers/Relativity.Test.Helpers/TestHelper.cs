using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.Logging;
using Relativity.Test.Helpers.ServiceFactory;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Relativity.Services.ServiceProxy;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models;


namespace Relativity.Test.Helpers
{
	public class TestHelper : IHelper
	{
		private readonly string _username;
		private readonly string _password;
		private readonly AppConfigSettings _alternateConfig;

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

		public TestHelper(TestContext testContext)
		{
			ConfigurationHelper.SetupConfiguration(testContext);
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
			throw new NotImplementedException();
		}

		public Guid GetGuid(int workspaceID, int artifactID)
		{
			ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(new Uri(ConfigurationHelper.RSAPI_SERVER_ADDRESS), new Uri(ConfigurationHelper.REST_SERVER_ADDRESS), new UsernamePasswordCredentials(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD));
			Services.ServiceProxy.ServiceFactory serviceFactory = new Services.ServiceProxy.ServiceFactory(serviceFactorySettings);

			Guid guid;
			using (ITestHelpersService testHelperService = serviceFactory.CreateProxy<ITestHelpersService>())
			{
				GetGuidModel ggModel = testHelperService.GetGuidAsync(artifactID, workspaceID).Result;
				guid = ggModel.Guid;
			}

			return guid;
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
