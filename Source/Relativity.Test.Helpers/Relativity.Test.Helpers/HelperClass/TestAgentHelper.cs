using Relativity.API;
using System;

namespace Relativity.Test.Helpers.HelperClasses
{
	public class TestAgentHelper : IAgentHelper
	{
		private IHelper _helper;
		private int WorkspaceID { get; set; }
		public string AdminUsername { get; set; }
		public string AdminPassword { get; set; }
		private string SQLServerAddress { get; set; }
		private string SQLUserName { get; set; }
		private string SQLPassword { get; set; }
		private string SolutionSnapshotRAPUrl { get; set; }
		private string ServerHostName { get; set; }
		private string ServerHostBinding { get; set; }
		private string RAPFilesDirectory { get; set; }

		public TestAgentHelper()
		{
			this._helper = new TestHelper(AdminUsername, AdminPassword);
		}

		void IDisposable.Dispose()
		{
			this._helper.Dispose();
		}

		IAuthenticationMgr IAgentHelper.GetAuthenticationManager()
		{
			throw new NotImplementedException();
		}

		IDBContext IHelper.GetDBContext(int caseID)
		{
			return this._helper.GetDBContext(caseID);
		}

		Guid IHelper.GetGuid(int workspaceID, int artifactID)
		{
			return this._helper.GetGuid(workspaceID, artifactID);
		}

		IInstanceSettingsBundle IHelper.GetInstanceSettingBundle()
		{
			return this._helper.GetInstanceSettingBundle();
		}

		ILogFactory IHelper.GetLoggerFactory()
		{
			return this._helper.GetLoggerFactory();
		}

		string IHelper.GetSchemalessResourceDataBasePrepend(IDBContext context)
		{
			return this._helper.GetSchemalessResourceDataBasePrepend(context);
		}

		ISecretStore IHelper.GetSecretStore()
		{
			return this._helper.GetSecretStore();
		}

		IServicesMgr IHelper.GetServicesManager()
		{
			return this._helper.GetServicesManager();
		}
		/* Needed for Relativity DLLS 10.* and above
        IStringSanitizer IHelper.GetStringSanitizer(int workspaceID)
        {
            return this._helper.GetStringSanitizer(workspaceID);
        } */

		IUrlHelper IHelper.GetUrlHelper()
		{
			return this._helper.GetUrlHelper();
		}

		string IHelper.ResourceDBPrepend()
		{
			return this._helper.ResourceDBPrepend();
		}

		string IHelper.ResourceDBPrepend(IDBContext context)
		{
			return this._helper.ResourceDBPrepend(context);
		}

		public IStringSanitizer GetStringSanitizer(int workspaceID)
		{
			throw new NotImplementedException();
		}
	}
}
