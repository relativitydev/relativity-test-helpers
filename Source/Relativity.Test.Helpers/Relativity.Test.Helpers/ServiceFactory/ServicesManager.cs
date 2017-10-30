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

        public ServicesManager(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public T CreateProxy<T>(ExecutionIdentity ident) where T : IDisposable
        {
            Credentials creds = null;
            if (ident == ExecutionIdentity.CurrentUser)
            {
                creds = new Relativity.Services.ServiceProxy.UsernamePasswordCredentials(_username, _password);
            }
            else if (ident == ExecutionIdentity.System)
            {
                var username = SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME;
                var password = SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD;
                creds = new UsernamePasswordCredentials(username, password);
            }

            if (creds == null)
            {
                throw new NotSupportedException($"{ident} is not supported in the Test Service Mangager.");
            }

            //Create the ServiceFactory with the given credentials and urls
            ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(GetServicesURL(), this.GetKeplerUrl(), creds);
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