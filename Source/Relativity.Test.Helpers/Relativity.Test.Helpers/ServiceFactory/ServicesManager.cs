using Relativity.API;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;

namespace Relativity.Test.Helpers.ServiceFactory
{
	public class ServicesManager : IServicesMgr
	{
		private readonly string _username;
		private readonly string _password;
		private readonly AppConfigSettings _alternateConfig;

		public ServicesManager(string username, string password)
		{
			_username = username;
			_password = password;
		}

		public ServicesManager(AppConfigSettings alternateConfig)
		{
			this._alternateConfig = alternateConfig;
		}

		public T CreateProxy<T>(ExecutionIdentity ident) where T : IDisposable
		{
			Credentials creds = null;

			if (this._alternateConfig != null)
			{
				creds = new UsernamePasswordCredentials(_alternateConfig.AdminUserName, _alternateConfig.AdminPassword);
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
				throw new NotSupportedException($"{ident} is not supported in the Test Service Manager.");
			}

			ServiceFactorySettings serviceFactorySettings;
			Relativity.Services.ServiceProxy.ServiceFactory serviceFactory;

			if (this._alternateConfig != null)
			{
				serviceFactorySettings = new ServiceFactorySettings(GetServicesURL(), this.GetKeplerUrl(this._alternateConfig), creds);
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
			Uri servicesUri;
			string relativityInstance = (string.IsNullOrEmpty(this._alternateConfig.RestServerAddress) ? this._alternateConfig.RelativityInstanceAddress : this._alternateConfig.RestServerAddress);

			// Get Services URL
			if (this._alternateConfig != null)
			{
				string serviceEndpoint = $"{this._alternateConfig.ServerBindingType}://{relativityInstance}/relativity.services";
				servicesUri = new Uri(serviceEndpoint);
			}
			else
			{
				servicesUri = new Uri($"{SharedTestHelpers.ConfigurationHelper.SERVER_BINDING_TYPE}://{SharedTestHelpers.ConfigurationHelper.RSAPI_SERVER_ADDRESS}/relativity.services");
			}
			return servicesUri;
		}


		public Uri GetServicesURL()
		{
			Uri servicesUri;
			string relativityInstance = (string.IsNullOrEmpty(this._alternateConfig.RsapiServerAddress) ? this._alternateConfig.RelativityInstanceAddress : this._alternateConfig.RsapiServerAddress);

			// Get Services URL
			if (this._alternateConfig != null)
			{
				string serviceEndpoint = $"{this._alternateConfig.ServerBindingType}://{relativityInstance}/relativity.services";
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