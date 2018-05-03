using System;
using System.Collections;
using System.Configuration;

namespace Relativity.Test.Helpers.SharedTestHelpers
{
    public class AppConfigSettings : ConfigurationSettings
    {
        public override string TestDataLocation
        {
            get { return ConfigurationManager.AppSettings["TestDataLocation"]; }
        }

        public override int WorkspaceId
        {
            get { return int.Parse(ConfigurationManager.AppSettings["WorkspaceID"]); }
        }

        public override string RsapiServerAddress
        {
            get { return ConfigurationManager.AppSettings["RSAPIServerAddress"]; }
        }

        public override string RestServerAddress
        {
            get { return ConfigurationManager.AppSettings["RESTServerAddress"]; }
        }

        public override string RelativityInstanceAddress
        {
            get { return ConfigurationManager.AppSettings["RelativityInstanceAddress"]; }
        }

        public override string AdminUserName
        {
            get { return ConfigurationManager.AppSettings["AdminUsername"]; }
        }

        public override string AdminPassword
        {
            get { return ConfigurationManager.AppSettings["AdminPassword"]; }
        }

        public override string SqlServerAddress
        {
            get { return ConfigurationManager.AppSettings["SQLServerAddress"]; }
        }

        public override string SqlUserName
        {
            get { return ConfigurationManager.AppSettings["SQLUsername"]; }
        }

        public override string SqlPassword
        {
            get { return ConfigurationManager.AppSettings["SQLPassword"]; }
        }

        public override string TestWorkspaceName
        {
            get { return ConfigurationManager.AppSettings["TestWorkspaceName"]; }
        }

        public override string TestWorkspaceTemplateName
        {
            get { return ConfigurationManager.AppSettings["TestWorkspaceTemplateName"]; }
        }

        public override string ServerBindingType
        {
            get { return ConfigurationManager.AppSettings["ServerBindingType"]; }
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
            get { return ConfigurationManager.AppSettings["DistSQLServerAddress"]; }
        }

        public override string UploadTestingServiceRap
        {
            get { return ConfigurationManager.AppSettings["UploadTestingServiceRap"]; }
        }

        public override string TestingServiceRapPath
        {
            get { return ConfigurationManager.AppSettings["TestingServiceRapPath"]; }
        }

        public override string AgentServerAddress
        {
            get { return ConfigurationManager.AppSettings["AgentServerAddress"] ?? "."; }
        }
    }
}
