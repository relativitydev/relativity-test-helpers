using System;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;

namespace Relativity.Test.Helpers.WorkspaceHelpers
{
	public class WorkspaceHelpers
	{
		public static string GetWorkspaceName(IRSAPIClient client, int workspaceArtifactId)
		{

			client.APIOptions.WorkspaceID = workspaceArtifactId;
			Int32 originalWorkspaceID = client.APIOptions.WorkspaceID;
			client.APIOptions.WorkspaceID = -1;
			Workspace workspace = client.Repositories.Workspace.ReadSingle(originalWorkspaceID);
			client.APIOptions.WorkspaceID = originalWorkspaceID;
			return workspace.Name;

		}
	}
}
