using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.API;
using Relativity.Test.Helpers.Authentication;
using Relativity.Test.Helpers.Authentication.Models;
using Relativity.Test.Helpers.Configuration.Models;

namespace Relativity.Test.Helpers
{
	public class TestAgentHelper : IAgentHelper
	{
		private TestHelper _helper;

		public TestAgentHelper()
		{
			this._helper = new TestHelper();
		}

		public TestAgentHelper(string username, string password)
		{
			this._helper = new TestHelper(username, password);
		}

		public TestAgentHelper(ConfigurationModel configs)
		{
			this._helper = new TestHelper(configs);
		}

		void IDisposable.Dispose()
		{
			this._helper.Dispose();
		}

		IAuthenticationMgr IAgentHelper.GetAuthenticationManager()
		{
			var userInfo = new UserInfo
			{
				FirstName = this._helper.Configs.FirstName,
				EmailAddress = this._helper.Configs.EmailAddress
			};
			var authManager = new AuthenticationManager(userInfo);
			return authManager;
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
