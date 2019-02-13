using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;

namespace Relativity.Test.Helpers.Objects.Workspace
{
	public class WorkspaceHelper
	{
		public static string GetWorkspaceName(IRSAPIClient client, int workspaceArtifactId)
		{
			var oldId = client.APIOptions.WorkspaceID;
			try
			{
				client.APIOptions.WorkspaceID = -1;
				kCura.Relativity.Client.DTOs.Workspace workspace = client.Repositories.Workspace.ReadSingle(workspaceArtifactId);
				return workspace.Name;
			}
			finally
			{
				client.APIOptions.WorkspaceID = oldId;
			}

		}
	}
}
