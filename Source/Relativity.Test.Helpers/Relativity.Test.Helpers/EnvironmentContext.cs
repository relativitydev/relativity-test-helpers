namespace Relativity.Test.Helpers
{
    public class EnvironmentContext
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }

        public string SqlServerUrl { get; set; }
        public string SqlUserName { get; set; }
        public string SqlPassword { get; set; }
        public string ServerBindingType { get; set; }
        public string ServerAddress { get; set; }

        public string RestServiceUrl
        {
            get
            {
                return $"{ServerBindingType}://{ServerAddress}/relativity.rest";
            }
        }
        public string ServicesUrl
        {
            get
            {
                return $"{ServerBindingType}://{ServerAddress}/relativity.services";
            }
        }

        public string KeplerUrl
        {
            get
            {
                return $"{RestServiceUrl}/api";
            }
        }

        public static EnvironmentContext FromConfig()
        {
            return new EnvironmentContext
            {
                Username = SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME,
                Password = SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD,
                SqlServerUrl = SharedTestHelpers.ConfigurationHelper.SQL_SERVER_ADDRESS,
                SqlUserName = SharedTestHelpers.ConfigurationHelper.SQL_USER_NAME,
                SqlPassword = SharedTestHelpers.ConfigurationHelper.SQL_PASSWORD,
                ServerBindingType = SharedTestHelpers.ConfigurationHelper.SERVER_BINDING_TYPE,
                ServerAddress = SharedTestHelpers.ConfigurationHelper.RSAPI_SERVER_ADDRESS
            };
        }
    }


}
