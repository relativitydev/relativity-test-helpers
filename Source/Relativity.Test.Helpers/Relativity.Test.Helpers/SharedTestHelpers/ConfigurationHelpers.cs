using System;
using System.Collections;
using System.Configuration;

namespace Relativity.Test.Helpers.SharedTestHelpers
{
	//I'd like to move away from this class since it creates a dependency on app.config
	//This is not always the case for all projects
	//the values that are relied upon here should be passed into the needed function
	[Obsolete("This should not be relied upon should be passed into the respected function")]
	public static class ConfigurationHelper
	{
		public static string TEST_DATA_LOCATION
		{
			get { return ConfigurationManager.AppSettings["TestDataLocation"]; }
		}

		public static Int32 WORKSPACEID
		{
			get { return Int32.Parse(ConfigurationManager.AppSettings["WorkspaceID"]); }
		}

		public static string RSAPI_SERVER_ADDRESS
		{
			get { return ConfigurationManager.AppSettings["RSAPIServerAddress"]; }
		}

		public static string REST_SERVER_ADDRESS
		{
			get { return ConfigurationManager.AppSettings["RESTServerAddress"]; }
		}

		public static string RELATIVITY_INSTANCE_ADDRESS
		{
			get { return ConfigurationManager.AppSettings["RelativityInstanceAddress"]; }
		}

		public static string ADMIN_USERNAME
		{
			get { return ConfigurationManager.AppSettings["AdminUsername"]; }
		}

		public static string DEFAULT_PASSWORD
		{
			get { return ConfigurationManager.AppSettings["AdminPassword"]; }
		}

		public static string SQL_SERVER_ADDRESS
		{
			get { return ConfigurationManager.AppSettings["SQLServerAddress"]; }
		}

		public static string SQL_USER_NAME
		{
			get { return ConfigurationManager.AppSettings["SQLUsername"]; }
		}

		public static string SQL_PASSWORD
		{
			get { return ConfigurationManager.AppSettings["SQLPassword"]; }
		}

		public static string TEST_WORKSPACE_NAME
		{
			get { return ConfigurationManager.AppSettings["TestWorkspaceName"]; }
		}

		public static string TEST_WORKSPACE_TEMPLATE_NAME
		{
			get { return ConfigurationManager.AppSettings["TestWorkspaceTemplateName"]; }
		}

		public static string SERVER_BINDING_TYPE
		{
			get { return ConfigurationManager.AppSettings["ServerBindingType"]; }
		}

		public static string SQL_CONNECTION_STRING
		{
			get
			{
				var section = ConfigurationManager.GetSection("kCura.Config") as Hashtable;
				return (string)section["connectionString"];
			}
		}

		public static string AUTH_TOKEN_RELADMIN
		{
			get
			{
				string credentialsString = string.Format("{0}:{1}", ADMIN_USERNAME, DEFAULT_PASSWORD);
				string credentialsB64 = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(credentialsString));
				string authHdr = string.Format("Basic {0}", credentialsB64);
				return authHdr;
			}
		}

		public static string DISTRIBUTED_SQL_ADDRESS
		{
			get { return ConfigurationManager.AppSettings["DistSQLServerAddress"]; }
		}

		public static string UPLOAD_TESTING_SERVICE_RAP
		{
			get { return ConfigurationManager.AppSettings["UploadTestingServiceRap"]; }
		}

		public static string TESTING_SERVICE_RAP_PATH
		{
			get { return ConfigurationManager.AppSettings["TestingServiceRapPath"]; }
		}

		public static string AGENT_SERVER_ADDRESS
		{
			get { return ConfigurationManager.AppSettings["AgentServerAddress"] ?? "."; }
		}

	}
}
