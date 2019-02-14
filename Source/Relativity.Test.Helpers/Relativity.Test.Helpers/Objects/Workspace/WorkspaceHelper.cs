using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using DTOs = kCura.Relativity.Client.DTOs;
using Relativity.API;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using System;
using System.Linq;
using System.Threading.Tasks;

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
			} catch (Exception ex)
			{

			}
			return workspace.Name;
		}

		public async Task<Int32> CreateWorkspaceAsync(string workspaceName, string templateName, string userName, string password)
		{
				return await Task.Run(() => Create(workspaceName, templateName)).ConfigureAwait(false);
		}

		/// <summary>
		/// Creates a new workspace based on a template workspace
		/// </summary>
		/// <param name="proxy">The RSAPI proxy to create the workspace</param>
		/// <param name="workspaceName">The name of the workspace that will get created</param>
		/// <param name="templateName">The template workspace that the new workspace will be based off of</param>
		/// <returns>Artifact Id of the created workspace</returns>
		public int Create(string workspaceName, string templateName)
		{
			return CreateInternal(workspaceName, templateName, null);
		}

		/// <summary>
		/// Creates a new workspace based on a template workspace
		/// </summary>
		/// <param name="proxy">The RSAPI proxy to create the workspace</param>
		/// <param name="workspaceName">The name of the workspace that will get created</param>
		/// <param name="templateName">The template workspace that the new workspace will be based off of</param>
		/// <param name="serverId">The server Id may not be correct and may need to be updatedServerID is hard-coded since we don't have a way to get the server ID.</param>
		/// <returns>Artifact Id of the created workspace</returns>
		public int Create(string workspaceName, string templateName, int serverId)
		{
			return CreateInternal(workspaceName, templateName, serverId);
		}

		private int CreateInternal(string workspaceName, string templateName, int? serverId)
		{
			int? testId = null;
			try
			{
				
				if (string.IsNullOrWhiteSpace(templateName))
				{
					throw new SystemException("Template name is blank in your configuration setting. Please add a template name to create a workspace");
				}
				var resultSet = GetArtifactIdOfTemplate(templateName);

				if (!resultSet.Success)
				{
					throw new ApplicationException($"Error creating workspace {workspaceName} with error {resultSet.Message}");
				}

				if (!resultSet.Results.Any())
				{
					throw new ApplicationException($"No template with name {templateName} found in this environment");
				}

				var workspace = resultSet.Results.FirstOrDefault().Artifact;
				int templateArtifactID = workspace.ArtifactID;

				var workspaceDTO = new kCura.Relativity.Client.DTOs.Workspace();
				workspaceDTO.Name = workspaceName;

				if (serverId.HasValue)
				{
					workspaceDTO.ServerID = serverId.Value;
				}

				ProcessOperationResult result;
				using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(API.ExecutionIdentity.System))
				{
					client.APIOptions.WorkspaceID = -1;
					result = client.Repositories.Workspace.CreateAsync(templateArtifactID, workspaceDTO);
				}
				if (!result.Success)
				{
					throw new System.Exception($"Workspace creation failed: {result.Message}");
				}
				ProcessInformation info;
				using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(API.ExecutionIdentity.System))
				{
					client.APIOptions.WorkspaceID = -1;
					info = client.GetProcessState(client.APIOptions, result.ProcessID);
				}
				int iteration = 0;

				//I have a feeling this will bite us in the future, but it hasn't yet
				while (info.State != ProcessStateValue.Completed)
				{
					//Workspace creation takes some time sleep until the workspaces is created and then get the artifact id of the new workspace
					System.Threading.Thread.Sleep(10000);
					using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(API.ExecutionIdentity.System))
					{
						client.APIOptions.WorkspaceID = -1;
						info = client.GetProcessState(client.APIOptions, result.ProcessID);
					}

					if (iteration > 6)
					{
						Console.WriteLine("Workspace creation timed out");
					}
					iteration++;
				}
				testId = info?.OperationArtifactIDs?.FirstOrDefault();
				if (!testId.HasValue)
				{
					throw new Exception("There was an error getting the created workspaceId");
				}
				return testId.Value;
			} catch (Exception ex)
			{

			}
			return testId.Value;
		}

		private QueryResultSet<kCura.Relativity.Client.DTOs.Workspace> GetArtifactIdOfTemplate(string templateName)
		{
			var query = new Query<kCura.Relativity.Client.DTOs.Workspace>();
			query.Condition = new kCura.Relativity.Client.TextCondition(FieldFieldNames.Name, TextConditionEnum.EqualTo, templateName);
			query.Fields = FieldValue.AllFields;
			QueryResultSet<kCura.Relativity.Client.DTOs.Workspace> resultSet;
			using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(API.ExecutionIdentity.System))
			{
				client.APIOptions.WorkspaceID = -1;
				resultSet = client.Repositories.Workspace.Query(query, 0);
			}
			return resultSet;
		}

		public bool Delete(IRSAPIClient proxy, int workspaceID)
		{
			var oldWorkspaceId = proxy.APIOptions.WorkspaceID;
			proxy.APIOptions.WorkspaceID = -1;
			try
			{
				//Create a Workspace Artifact and pass to the Delete method on the repository
				var workspaceDTO = new kCura.Relativity.Client.DTOs.Workspace(workspaceID);
				DTOs.WriteResultSet<DTOs.Workspace> resultSet;
				using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(API.ExecutionIdentity.System))
				{
					client.APIOptions.WorkspaceID = -1;
					resultSet = proxy.Repositories.Workspace.Delete(workspaceDTO);
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
