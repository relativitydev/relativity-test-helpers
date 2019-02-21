using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using Relativity.Services.Workspace;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using System;
using System.Linq;
using System.Threading.Tasks;

using DTOs = kCura.Relativity.Client.DTOs;

namespace Relativity.Test.Helpers.Objects.Workspace
{
	public class WorkspaceHelper
	{
		private TestHelper _helper;

		public WorkspaceHelper(TestHelper helper)
		{
			_helper = helper;
		}

		public string GetWorkspaceName(int workspaceArtifactId)
		{
			kCura.Relativity.Client.DTOs.Workspace workspace = new DTOs.Workspace();
			try
			{
				using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(API.ExecutionIdentity.System))
				{
					client.APIOptions.WorkspaceID = -1;
					workspace = client.Repositories.Workspace.ReadSingle(workspaceArtifactId);
				}
			}
			catch (Exception ex)
			{
			}
			return workspace.Name;
		}

		public int Create(string workspaceName, int clientArtifactID, int matterArtifactID, DTOs.Workspace templateWorkspace)
		{
			int workspaceID;

			using (var workspaceManager = _helper.GetServicesManager().CreateProxy<IWorkspaceManager>(ExecutionIdentity.System))
			{
				var settings = new WorkspaceSetttings
				{
					Name = workspaceName,
					ClientArtifactId = clientArtifactID,
					MatterArtifactId = matterArtifactID,
					TemplateArtifactId = templateWorkspace.ArtifactID,
					StatusCodeArtifactId = 675,
					ResourceGroupArtifactId = templateWorkspace.ResourcePoolID,
					DefaultFileLocationCodeArtifactId = templateWorkspace.DefaultFileLocation.ArtifactID,
					DefaultDataGridLocationCodeArtifactId = templateWorkspace.DefaultDataGridLocation?.ArtifactID,
					DefaultCacheLocationServerArtifactId = templateWorkspace.DefaultCacheLocation,
				};
				WorkspaceRef result = workspaceManager.CreateWorkspaceAsync(settings).Result;
				workspaceID = result.ArtifactID;

				return workspaceID;
			}
		}

		public DTOs.Workspace ReadTemplateWorkspace()
		{
			DTOs.Workspace templateWorkspace;

			using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.CurrentUser))
			{
				client.APIOptions.WorkspaceID = -1;

				templateWorkspace = client.Repositories.Workspace.ReadSingle(1015024);
			}

			return templateWorkspace;
		}

		public bool Delete(int workspaceID)
		{
			try
			{
				//Create a Workspace Artifact and pass to the Delete method on the repository
				var workspaceDTO = new kCura.Relativity.Client.DTOs.Workspace(workspaceID);
				DTOs.WriteResultSet<DTOs.Workspace> resultSet;
				using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(API.ExecutionIdentity.System))
				{
					client.APIOptions.WorkspaceID = -1;
					resultSet = client.Repositories.Workspace.Delete(workspaceDTO);
				}
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
			return true;
		}
	}
}