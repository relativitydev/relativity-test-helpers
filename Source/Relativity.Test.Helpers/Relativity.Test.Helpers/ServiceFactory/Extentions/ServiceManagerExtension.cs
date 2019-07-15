using Relativity.API;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;

namespace Relativity.Test.Helpers.ServiceFactory.Extentions
{
	public static class ServiceManagerExtension
	{
		public static T GetProxy<T>(this IServicesMgr svcmgr, string username, string password) where T : IDisposable
		{
			//Create the ServiceFactory with the given credentials and urls
			ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(svcmgr.GetServicesURL(), svcmgr.GetKeplerUrl(), new Relativity.Services.ServiceProxy.UsernamePasswordCredentials(username, password));
			Relativity.Services.ServiceProxy.ServiceFactory serviceFactory = new Relativity.Services.ServiceProxy.ServiceFactory(serviceFactorySettings);
			//Create proxy
			T proxy = serviceFactory.CreateProxy<T>();
			return proxy;

		}

		public static Uri GetKeplerUrl(this IServicesMgr svcmgr)
		{
			// Get Kepler URL
			Uri keplerUri = new Uri($"{SharedTestHelpers.ConfigurationHelper.SERVER_BINDING_TYPE}://{SharedTestHelpers.ConfigurationHelper.REST_SERVER_ADDRESS}/relativity.rest/api");
			return keplerUri;
		}

		public static Uri GetKeplerUrl(this IServicesMgr svcmgr, AppConfigSettings alternateConfig)
		{
			// Get Kepler URL
			Uri keplerUri = new Uri($"{alternateConfig.ServerBindingType}://{alternateConfig.RelativityInstanceAddress}/relativity.rest/api");
			return keplerUri;
		}
	}
}