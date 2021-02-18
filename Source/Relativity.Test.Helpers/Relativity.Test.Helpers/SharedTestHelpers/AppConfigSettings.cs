using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace Relativity.Test.Helpers.SharedTestHelpers
{
	public class AppConfigSettings : ConfigurationSettings
	{
		private readonly NameValueCollection _appSettings;

		public AppConfigSettings()
		{
			_appSettings = ConfigurationManager.AppSettings;
		}

		public AppConfigSettings(string configSectionName)
		{
			_appSettings = (NameValueCollection)ConfigurationManager.GetSection(configSectionName);
		}

		public AppConfigSettings(Dictionary<string, string> configDictionary)
		{
			_appSettings = new NameValueCollection();

			foreach (var keyValuePair in configDictionary)
			{
				_appSettings.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public override string TestDataLocation
		{
			get { return _appSettings["TestDataLocation"]; }
		}

		public override int WorkspaceId
		{
			get { return int.Parse(_appSettings["WorkspaceID"]); }
		}

		public override string RsapiServerAddress
		{
			get { return _appSettings["RSAPIServerAddress"]; }
		}

		public override string RestServerAddress
		{
			get { return _appSettings["RESTServerAddress"]; }
		}

		public override string RelativityInstanceAddress
		{
			get { return _appSettings["RelativityInstanceAddress"]; }
		}

		public override string AdminUserName
		{
			get { return _appSettings["AdminUsername"]; }
		}

		public override string AdminPassword
		{
			get { return _appSettings["AdminPassword"]; }
		}

		public override string SqlServerAddress
		{
			get { return _appSettings["SQLServerAddress"]; }
		}

		public override string SqlUserName
		{
			get { return _appSettings["SQLUsername"]; }
		}

		public override string SqlPassword
		{
			get { return _appSettings["SQLPassword"]; }
		}

		public override string TestWorkspaceName
		{
			get { return _appSettings["TestWorkspaceName"]; }
		}

		public override string TestWorkspaceTemplateName
		{
			get { return _appSettings["TestWorkspaceTemplateName"]; }
		}

		public override string ServerBindingType
		{
			get { return _appSettings["ServerBindingType"]; }
		}

		public override string SqlConnectionString
		{
			get
			{
				var section = ConfigurationManager.GetSection("kCura.Config") as Hashtable;
				return (string)section["connectionString"];
			}
		}

		public override string RelAdminAuthToken
		{
			get
			{
				string credentialsString = string.Format("{0}:{1}", this.AdminUserName, this.AdminPassword);
				string credentialsB64 = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(credentialsString));
				string authHdr = string.Format("Basic {0}", credentialsB64);
				return authHdr;
			}
		}

		public override string DistributedSqlAddress
		{
			get { return _appSettings["DistSQLServerAddress"]; }
		}

		public override string UploadTestingServiceRap
		{
			get { return _appSettings["UploadTestingServiceRap"]; }
		}

		public override string TestingServiceRapPath
		{
			get { return _appSettings["TestingServiceRapPath"]; }
		}

		public override string AgentServerAddress
		{
			get { return _appSettings["AgentServerAddress"] ?? "."; }
		}

		public override string ForceDbContext
		{
			get { return _appSettings["ForceDbContext"]; }
		}
	}
}
