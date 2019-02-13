using kCura.Relativity.Client;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using System;
//using IServicesMgr = Relativity.Test.Helpers.Interface.IServicesMgr;
using IServicesMgr = Relativity.API.IServicesMgr;

namespace Relativity.Test.Helpers.WorkspaceHelpers
{
	/// <summary>
	/// 
	/// DeleteTestWorkspace assists in deleting Workspaces for tests
	/// 
	/// </summary>
	public static class DeleteWorkspace
	{
		public static bool Delete(IRSAPIClient proxy, int workspaceID)
		{
			var oldWorkspaceId = proxy.APIOptions.WorkspaceID;
			proxy.APIOptions.WorkspaceID = -1;
			try
			{
				//Create a Workspace Artifact and pass to the Delete method on the repository
				var workspaceDTO = new kCura.Relativity.Client.DTOs.Workspace(workspaceID);
				var resultSet = proxy.Repositories.Workspace.Delete(workspaceDTO);
				if (!resultSet.Success)
				{
					Console.WriteLine("An error occurred deleting the Workspace: {0}", resultSet.Message);
					return false;
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine("An error occurred deleting the Workspace: {0}", ex.Message);
				return false;
			}
			finally
			{
				proxy.APIOptions.WorkspaceID = oldWorkspaceId;
			}
			return true;
		}

		public static bool DeleteTestWorkspace(Int32 workspaceID, IServicesMgr svcMgr, string userName, string password)
		{
			using (var proxy = svcMgr.GetProxy<IRSAPIClient>(new Models.ConfigurationModel()))
			{
				return Delete(proxy, workspaceID);
			}
		}
	}
}
