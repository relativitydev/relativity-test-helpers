using System.Collections.Generic;

namespace Relativity.Test.Helpers.SharedTestHelpers
{
	public static class ConfigurationHelper
	{
		private static ConfigurationSettings _settings;
		public static ConfigurationSettings Default
		{
			get
			{
				return new AppConfigSettings();
			}
		}
		static ConfigurationHelper()
		{
			SetupConfiguration(Default);
		}
		public static void SetupConfiguration(ConfigurationSettings settings)
		{
			_settings = settings;
		}

		public static void SetupConfiguration(Dictionary<string, string> configDictionary)
		{
			_settings = new AppConfigSettings(configDictionary);
		}

		public static string TEST_DATA_LOCATION
		{
			get { return _settings.TestDataLocation; }
		}

		public static int WORKSPACEID
		{
			get { return _settings.WorkspaceId; }
		}

		public static string REST_SERVER_ADDRESS
		{
			get { return _settings.RestServerAddress; }
		}

		public static string RELATIVITY_INSTANCE_ADDRESS
		{
			get { return _settings.RelativityInstanceAddress; }
		}

		public static string ADMIN_USERNAME
		{
			get { return _settings.AdminUserName; }
		}

		public static string DEFAULT_PASSWORD
		{
			get { return _settings.AdminPassword; }
		}

		public static string SQL_SERVER_ADDRESS
		{
			get { return _settings.SqlServerAddress; }
		}

		public static string SQL_USER_NAME
		{
			get { return _settings.SqlUserName; }
		}

		public static string SQL_PASSWORD
		{
			get { return _settings.SqlPassword; }
		}

		public static string TEST_WORKSPACE_NAME
		{
			get { return _settings.TestWorkspaceName; }
		}

		public static string TEST_WORKSPACE_TEMPLATE_NAME
		{
			get { return _settings.TestWorkspaceTemplateName; }
		}

		public static string SERVER_BINDING_TYPE
		{
			get { return _settings.ServerBindingType; }
		}

		public static string SQL_CONNECTION_STRING
		{
			get
			{
				return _settings.SqlConnectionString;
			}
		}

		public static string AUTH_TOKEN_RELADMIN
		{
			get
			{
				return _settings.RelAdminAuthToken;
			}
		}

		public static string DISTRIBUTED_SQL_ADDRESS
		{
			get { return _settings.DistributedSqlAddress; }
		}

		public static string UPLOAD_TESTING_SERVICE_RAP
		{
			get { return _settings.UploadTestingServiceRap; }
		}

		public static string TESTING_SERVICE_RAP_PATH
		{
			get { return _settings.TestingServiceRapPath; }
		}

		public static string AGENT_SERVER_ADDRESS
		{
			get { return _settings.AgentServerAddress; }
		}

	}
}
