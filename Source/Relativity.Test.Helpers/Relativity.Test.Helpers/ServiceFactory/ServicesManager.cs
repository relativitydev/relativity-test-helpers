using Relativity.API;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using Relativity.Test.Helpers.Configuration.Models;
using System;

namespace Relativity.Test.Helpers.ServiceFactory
{
	public class ServicesManager : IServicesMgr
	{
		private readonly ConfigurationModel _configs = null;
		private readonly string _username;
		private readonly string _password;

		public ServicesManager(string username, string password)
		{
			_username = username;
			_password = password;
		}

		public ServicesManager(ConfigurationModel configs)
		{
			this._configs = configs;
		}

		public T CreateProxy<T>(ExecutionIdentity ident) where T : IDisposable
		{
			Credentials creds = null;
			if (this._configs != null)
			{
				creds = new UsernamePasswordCredentials(_configs.AdminUsername, _configs.AdminPassword);
			}
			else
			{
				if (ident == ExecutionIdentity.CurrentUser)
				{
					creds = new UsernamePasswordCredentials(_username, _password);
				}
				else if (ident == ExecutionIdentity.System)
				{
					var username = SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME;
					var password = SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD;
					creds = new UsernamePasswordCredentials(username, password);
				}
			}

			if (creds == null)
			{
				throw new NotSupportedException($"{ident} is not supported in the Test Service Mangager.");
			}

			ServiceFactorySettings serviceFactorySettings;
			Relativity.Services.ServiceProxy.ServiceFactory serviceFactory;

			if (this._configs != null)
			{
				serviceFactorySettings = new ServiceFactorySettings(GetServicesURL(), this.GetKeplerUrl(this._configs), creds);
				serviceFactory = new Relativity.Services.ServiceProxy.ServiceFactory(serviceFactorySettings);
			}
			else
			{
				serviceFactorySettings = new ServiceFactorySettings(GetServicesURL(), this.GetKeplerUrl(), creds);
				serviceFactory = new Relativity.Services.ServiceProxy.ServiceFactory(serviceFactorySettings);
			}
			T proxy = serviceFactory.CreateProxy<T>();
			return proxy;
		}

		public Uri GetRESTServiceUrl()
		{
			Uri servicesUrl = new Uri($"{SharedTestHelpers.ConfigurationHelper.SERVER_BINDING_TYPE}://{SharedTestHelpers.ConfigurationHelper.RSAPI_SERVER_ADDRESS}/relativity.rest");
			return servicesUrl;
		}


		public Uri GetServicesURL()
		{
			Uri servicesUri;
			// Get Services URL
			if (this._configs != null)
			{
				string serviceEndpoint = $"{this._configs.ServerHostBinding}://{this._configs.ServerHostName}/relativity.services";
				servicesUri = new Uri(serviceEndpoint);
			}
			else
			{
				servicesUri = new Uri($"{SharedTestHelpers.ConfigurationHelper.SERVER_BINDING_TYPE}://{SharedTestHelpers.ConfigurationHelper.RSAPI_SERVER_ADDRESS}/relativity.services");
			}
			return servicesUri;
		}
	}
}