using Relativity.API;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.Configuration.Models;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using System;

namespace Relativity.Test.Helpers.ServiceFactory
{
	internal class ServicesManager : IServicesMgr
	{
		private readonly ConfigurationModel _configs = null;

		public ServicesManager(ConfigurationModel configs)
		{
			this._configs = configs;
		}

		public T CreateProxy<T>(ExecutionIdentity ident) where T : IDisposable
		{
			Credentials creds = null;

			creds = new UsernamePasswordCredentials(_configs.AdminUsername, _configs.AdminPassword);

			ServiceFactorySettings serviceFactorySettings;
			Relativity.Services.ServiceProxy.ServiceFactory serviceFactory;

			serviceFactorySettings = new ServiceFactorySettings(GetServicesURL(), this.GetKeplerUrl(this._configs), creds);
			serviceFactory = new Relativity.Services.ServiceProxy.ServiceFactory(serviceFactorySettings);
			T proxy = serviceFactory.CreateProxy<T>();
			return proxy;
		}

		public Uri GetRESTServiceUrl()
		{
			Uri servicesUrl = new Uri($"{_configs.ServerHostBinding}://{_configs.ServerHostName}/relativity.rest");
			return servicesUrl;
		}

		public Uri GetServicesURL()
		{
			Uri servicesUri;

			string serviceEndpoint = $"{this._configs.ServerHostBinding}://{this._configs.ServerHostName}/relativity.services";
			servicesUri = new Uri(serviceEndpoint);

			return servicesUri;
		}
	}
}