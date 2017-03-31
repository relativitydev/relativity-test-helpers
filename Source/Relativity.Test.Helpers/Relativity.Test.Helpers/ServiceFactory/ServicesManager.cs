using System;
using Relativity.Services.ServiceProxy;
using IServicesMgr = Relativity.Test.Helpers.Interface.IServicesMgr;

namespace Relativity.Test.Helpers.ServiceFactory
{
	public class ServicesManager : IServicesMgr
	{
		string _username = string.Empty;
		string _password = string.Empty;

		public ServicesManager()
		{
			_username = SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME;
			_password = SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD;
		}

		public T GetProxy<T>(string username, string password) where T : IDisposable
		{
			//Create the ServiceFactory with the given credentials and urls
			ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(GetServicesURL(), GetKeplerUrl(), new Relativity.Services.ServiceProxy.UsernamePasswordCredentials(username, password));
			Relativity.Services.ServiceProxy.ServiceFactory serviceFactory = new Relativity.Services.ServiceProxy.ServiceFactory(serviceFactorySettings);
			//Create proxy
			T proxy = serviceFactory.CreateProxy<T>();
			return proxy;
		}

		public static Uri GetRESTServiceUrl()
		{
			Uri servicesUrl = new Uri(string.Format("{0}://{1}/relativity.rest", SharedTestHelpers.ConfigurationHelper.SERVER_BINDING_TYPE, SharedTestHelpers.ConfigurationHelper.RSAPI_SERVER_ADDRESS));
			return servicesUrl;
		}


		public static Uri GetServicesURL()
		{
			// Get Services URL
			var servicesUri = new Uri(string.Format("{0}://{1}/relativity.services", SharedTestHelpers.ConfigurationHelper.SERVER_BINDING_TYPE,SharedTestHelpers.ConfigurationHelper.RSAPI_SERVER_ADDRESS));
			return servicesUri;
		}

		public static Uri GetKeplerUrl()
		{
			// Get Kepler URL
			Uri keplerUri = new Uri(string.Format("{0}://{1}/relativity.rest/api", SharedTestHelpers.ConfigurationHelper.SERVER_BINDING_TYPE,SharedTestHelpers.ConfigurationHelper.REST_SERVER_ADDRESS));
			return keplerUri;
		}

	}
}