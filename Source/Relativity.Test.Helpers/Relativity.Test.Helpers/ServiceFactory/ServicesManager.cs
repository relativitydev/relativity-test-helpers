using Relativity.API;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using System;

namespace Relativity.Test.Helpers.ServiceFactory
{
    public class ServicesManager : IServicesMgr
    {
        private readonly string  _username;
        private readonly string  _password;

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
                creds = new UsernamePasswordCredentials(_username, _password);
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

            var serviceFactorySettings = new ServiceFactorySettings(GetServicesURL(), this.GetKeplerUrl(), creds);
            var serviceFactory = new Relativity.Services.ServiceProxy.ServiceFactory(serviceFactorySettings);
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