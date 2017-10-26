using System;
using Relativity.Services.ServiceProxy;
using Relativity.API;
//using IServicesMgr = Relativity.Test.Helpers.Interface.IServicesMgr;
using Relativity.Test.Helpers.ServiceFactory.Extentions;

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
			ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(GetServicesURL(), this.GetKeplerUrl(), new Relativity.Services.ServiceProxy.UsernamePasswordCredentials(username, password));
			Relativity.Services.ServiceProxy.ServiceFactory serviceFactory = new Relativity.Services.ServiceProxy.ServiceFactory(serviceFactorySettings);
			//Create proxy
			T proxy = serviceFactory.CreateProxy<T>();
			return proxy;
		}


        public T CreateProxy<T>(ExecutionIdentity ident) where T : IDisposable
        {
            //Could do something here with the different Security contexts.  I.E.  If ExecutionIdentity.System then use SharedTestHelpers.ConfigurationHelper.SYSTEM_USER_NAME and SharedTestHelpers.ConfigurationHelper.SYSTEM_PASSWORD
            //          and if ExecutionIdentity.CurrentUser then SharedTestHelpers.ConfigurationHelper.STANDARD_USER_NAME, etc

            //Create the ServiceFactory with the given credentials and urls
            ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(GetServicesURL(), this.GetKeplerUrl(), new Relativity.Services.ServiceProxy.UsernamePasswordCredentials(_username, _password));
            Relativity.Services.ServiceProxy.ServiceFactory serviceFactory = new Relativity.Services.ServiceProxy.ServiceFactory(serviceFactorySettings);
            //Create proxy
            T proxy = serviceFactory.CreateProxy<T>();
            return proxy;
        }

        public Uri GetRESTServiceUrl()
        {
            Uri servicesUrl = new Uri(string.Format("{0}://{1}/relativity.rest", SharedTestHelpers.ConfigurationHelper.SERVER_BINDING_TYPE, SharedTestHelpers.ConfigurationHelper.RSAPI_SERVER_ADDRESS));
            return servicesUrl;
        }


        public Uri GetServicesURL()
        {
            // Get Services URL
            var servicesUri = new Uri(string.Format("{0}://{1}/relativity.services", SharedTestHelpers.ConfigurationHelper.SERVER_BINDING_TYPE, SharedTestHelpers.ConfigurationHelper.RSAPI_SERVER_ADDRESS));
            return servicesUri;
        }
    }
}