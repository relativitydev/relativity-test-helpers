using System;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
//using IServicesMgr = Relativity.Test.Helpers.Interface.IServicesMgr;
using IServicesMgr = Relativity.API.IServicesMgr;
using Relativity.Test.Helpers.ServiceFactory.Extentions;

namespace Relativity.Test.Helpers.WorkspaceHelpers
{
	/// <summary>
	/// 
	/// DeleteTestWorkspace assists in deleting Workspaces for tests
	/// 
	/// </summary>
	public static class DeleteWorkspace
	{
		public static bool DeleteTestWorkspace(Int32 workspaceID, IServicesMgr svcMgr, string userName, string password)
		{
			WriteResultSet<Workspace> resultSet = null;
			using (var proxy = svcMgr.GetProxy<IRSAPIClient>(userName, password))
			{
				proxy.APIOptions.WorkspaceID = -1;

				try
				{
					//Create a Workspace Artifact and pass to the Delete method on the repository
					var workspaceDTO = new kCura.Relativity.Client.DTOs.Workspace(workspaceID);
					resultSet = proxy.Repositories.Workspace.Delete(workspaceDTO);
				}
				catch (Exception ex)
				{
					Console.WriteLine("An error occurred deleting the Workspace: {0}", ex.Message);
					return false;
				}


				if (!resultSet.Success)
				{
					Console.WriteLine("An error occurred deleting the Workspace: {0}", resultSet.Message);
					return false;
				}

				return true;
			}
		}

	}
}
