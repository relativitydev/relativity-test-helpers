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
using Newtonsoft.Json.Linq;
using Relativity.Services.ServiceProxy;


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
			HttpClient httpClient = new HttpClient();
			string restAddress = ConfigurationHelper.SERVER_BINDING_TYPE + "://" + ConfigurationHelper.REST_SERVER_ADDRESS;
			string usernamePassword = string.Format("{0}:{1}", ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			string base64usernamePassword = Convert.ToBase64String(Encoding.ASCII.GetBytes(usernamePassword));
			httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64usernamePassword);
			httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", string.Empty);

			string request = "{\"artifactID\":\"@artifactID\",\"workspaceID\":\"@workspaceID\"}";
			request = request.Replace("@artifactID", artifactID.ToString());
			request = request.Replace("@workspaceID", workspaceID.ToString());
			string endpointUrl = restAddress + "/Relativity.REST/api/TestHelpersModule/v1/TestHelpersService/GetGuid";
			StringContent content = new StringContent(request);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = httpClient.PostAsync(endpointUrl, content).Result;
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Get Artifact Guid");
			}

			string result = response.Content.ReadAsStringAsync().Result;
			JToken resultObject = JObject.Parse(result);
			string guidString = resultObject["Guid"].Value<string>();
			Guid guid = new Guid(guidString);
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
