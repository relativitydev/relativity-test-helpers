using Relativity.API;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.Models;
using System;

namespace Relativity.Test.Helpers.ServiceFactory.Extentions
{
	public static class ServiceManagerExtension
	{
		public static T GetProxy<T>(this IServicesMgr svcmgr, ConfigurationModel configs) where T : IDisposable
		{
			//Create the ServiceFactory with the given credentials and urls
			ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(svcmgr.GetServicesURL(), svcmgr.GetKeplerUrl(configs), new Relativity.Services.ServiceProxy.UsernamePasswordCredentials(configs.AdminUsername, configs.AdminPassword));
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

		public static Uri GetKeplerUrl(this IServicesMgr svcmgr, ConfigurationModel configs)
		{
			// Get Kepler URL
			Uri keplerUri = new Uri($"{configs.ServerHostBinding}://{configs.ServerHostName}/relativity.rest/api");
			return keplerUri;
		}
	}


}
