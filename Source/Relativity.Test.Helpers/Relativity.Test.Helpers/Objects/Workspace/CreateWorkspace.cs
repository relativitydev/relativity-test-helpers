using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using System;
using System.Linq;
using System.Threading.Tasks;
using IServicesMgr = Relativity.API.IServicesMgr;

namespace Relativity.Test.Helpers.Objects.Workspace
{

	/// <summary>
	/// 
	/// CreateWorkspaceAsync assists in creating Workspaces for tests
	/// 
	/// </summary>

	public class CreateWorkspace
	{
		public async static Task<Int32> CreateWorkspaceAsync(string workspaceName, string templateName, IServicesMgr svcMgr, string userName, string password)
		{
			using (var client = svcMgr.GetProxy<IRSAPIClient>(new Configuration.Models.ConfigurationModel()))
			{
				client.APIOptions.WorkspaceID = -1;

				return await Task.Run(() => Create(client, workspaceName, templateName));
			}
		}

		/// <summary>
		/// Creates a new workspace based on a template workspace
		/// </summary>
		/// <param name="proxy">The RSAPI proxy to create the workspace</param>
		/// <param name="workspaceName">The name of the workspace that will get created</param>
		/// <param name="templateName">The template workspace that the new workspace will be based off of</param>
		/// <returns>Artifact Id of the created workspace</returns>
		public static int Create(IRSAPIClient proxy, string workspaceName, string templateName)
		{
			return CreateInternal(proxy, workspaceName, templateName, null);
		}

		/// <summary>
		/// Creates a new workspace based on a template workspace
		/// </summary>
		/// <param name="proxy">The RSAPI proxy to create the workspace</param>
		/// <param name="workspaceName">The name of the workspace that will get created</param>
		/// <param name="templateName">The template workspace that the new workspace will be based off of</param>
		/// <param name="serverId">The server Id may not be correct and may need to be updatedServerID is hard-coded since we don't have a way to get the server ID.</param>
		/// <returns>Artifact Id of the created workspace</returns>
		public static int Create(IRSAPIClient proxy, string workspaceName, string templateName, int serverId)
		{
			return CreateInternal(proxy, workspaceName, templateName, serverId);
		}

		private static int CreateInternal(IRSAPIClient proxy, string workspaceName, string templateName, int? serverId)
		{
			var oldWorkspaceId = proxy.APIOptions.WorkspaceID;
			try
			{
				proxy.APIOptions.WorkspaceID = -1;

				if (string.IsNullOrWhiteSpace(templateName))
				{
					throw new SystemException("Template name is blank in your configuration setting. Please add a template name to create a workspace");
				}
				var resultSet = GetArtifactIdOfTemplate(proxy, templateName);

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

				var result = proxy.Repositories.Workspace.CreateAsync(templateArtifactID, workspaceDTO);

				if (!result.Success)
				{
					throw new System.Exception($"Workspace creation failed: {result.Message}");
				}

				ProcessInformation info = proxy.GetProcessState(proxy.APIOptions, result.ProcessID);

				int iteration = 0;

				//I have a feeling this will bite us in the future, but it hasn't yet
				while (info.State != ProcessStateValue.Completed)
				{
					//Workspace creation takes some time sleep until the workspaces is created and then get the artifact id of the new workspace
					System.Threading.Thread.Sleep(10000);
					info = proxy.GetProcessState(proxy.APIOptions, result.ProcessID);

					if (iteration > 6)
					{
						Console.WriteLine("Workspace creation timed out");
					}
					iteration++;
				}
				var testId = info?.OperationArtifactIDs?.FirstOrDefault();
				if (!testId.HasValue)
				{
					throw new Exception("There was an error getting the created workspaceId");
				}
				return testId.Value;
			}
			finally
			{
				proxy.APIOptions.WorkspaceID = oldWorkspaceId;
			}
		}

		private static QueryResultSet<kCura.Relativity.Client.DTOs.Workspace> GetArtifactIdOfTemplate(IRSAPIClient proxy, string templateName)
		{
			var query = new Query<kCura.Relativity.Client.DTOs.Workspace>();
			query.Condition = new kCura.Relativity.Client.TextCondition(FieldFieldNames.Name, TextConditionEnum.EqualTo, templateName);
			query.Fields = FieldValue.AllFields;
			QueryResultSet<kCura.Relativity.Client.DTOs.Workspace> resultSet = proxy.Repositories.Workspace.Query(query, 0);
			return resultSet;
		}
	}
}
