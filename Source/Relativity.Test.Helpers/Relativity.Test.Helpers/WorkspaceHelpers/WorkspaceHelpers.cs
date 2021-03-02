using Relativity.API;
using Relativity.Services;
using Relativity.Services.Interfaces.Workspace;
using Relativity.Services.Interfaces.Workspace.Models;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Test.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NumericConditionEnum = Relativity.Services.NumericConditionEnum;
using QueryResult = Relativity.Services.Objects.DataContracts.QueryResult;
using TextConditionEnum = Relativity.Services.TextConditionEnum;

namespace Relativity.Test.Helpers.WorkspaceHelpers
{
	public class WorkspaceHelpers
	{
		#region Query

		/// <summary>
		/// Gets a workspace name for a given workspace ID
		/// </summary>
		/// <param name="servicesMgr"></param>
		/// <param name="workspaceArtifactId"></param>
		/// <returns></returns>
		public static string GetWorkspaceName(IServicesMgr servicesMgr, int workspaceArtifactId)
		{
			try
			{
				using (IObjectManager objectManager = servicesMgr.CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
				{
					QueryRequest queryRequest = new QueryRequest()
					{
						ObjectType = new ObjectTypeRef { ArtifactTypeID = Constants.ArtifactTypeIds.Workspace },
						Condition = new WholeNumberCondition("ArtifactId", NumericConditionEnum.EqualTo, workspaceArtifactId).ToQueryString(),
						Fields = new List<FieldRef>()
						{
							new FieldRef { Name = "Name" }
						},
					};
					QueryResult result = objectManager.QueryAsync(-1, queryRequest, 1, 10).Result;

					return result.Objects.First().FieldValues.Find(x => x.Field.Name == "Name").Value.ToString();
				}
			}
			catch (Exception ex)
			{
				string errorMessage = $"Could not find workspace name in {nameof(GetWorkspaceName)} for {nameof(workspaceArtifactId)} of {workspaceArtifactId} - {ex.Message}";
				Console.WriteLine(errorMessage);
				throw new TestHelpersException(errorMessage);
			}
		}

		/// <summary>
		/// Gets a workspace ID for a given workspace name
		/// </summary>
		/// <param name="servicesMgr"></param>
		/// <param name="workspaceName"></param>
		/// <returns></returns>
		public static async Task<int> GetWorkspaceIdByNameAsync(IServicesMgr servicesMgr, string workspaceName)
		{
			try
			{
				using (IObjectManager objectManager = servicesMgr.CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
				{
					QueryRequest queryRequest = new QueryRequest()
					{
						ObjectType = new ObjectTypeRef { ArtifactTypeID = Constants.ArtifactTypeIds.Workspace },
						Condition = new TextCondition("Name", TextConditionEnum.EqualTo, workspaceName).ToQueryString(),
						Fields = new List<FieldRef>()
						{
							new FieldRef { Name = "Name" }
						},
					};
					QueryResult result = await objectManager.QueryAsync(-1, queryRequest, 1, 10);

					return result.Objects.First().ArtifactID;
				}
			}
			catch (Exception ex)
			{
				string errorMessage = $"Could not find Workspace in {nameof(GetWorkspaceIdByNameAsync)} for {nameof(workspaceName)} of {workspaceName} - {ex.Message}";
				Console.WriteLine(errorMessage);
				throw new TestHelpersException(errorMessage);
			}
		}

		#endregion


		#region Create

		/// <summary>
		/// Creates a new workspace based on a template workspace
		/// </summary>
		/// <param name="servicesMgr"></param>
		/// <param name="workspaceName">The name of the workspace that will get created</param>
		/// <param name="templateName">The template workspace that the new workspace will be based off of</param>
		/// <returns>Artifact Id of the created workspace</returns>
		public static async Task<int> CreateAsync(IServicesMgr servicesMgr, string workspaceName, string templateName)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(templateName))
				{
					throw new SystemException(
						"Template name is blank in your configuration setting. Please add a template name to create a workspace");
				}

				int workspaceId;
				int templateWorkspaceId = await GetWorkspaceIdByNameAsync(servicesMgr, templateName);

				using (IWorkspaceManager workspaceManager = servicesMgr.CreateProxy<IWorkspaceManager>(ExecutionIdentity.CurrentUser))
				{
					WorkspaceResponse templateWorkspaceResponse = await workspaceManager.ReadAsync(templateWorkspaceId);

					WorkspaceRequest workspaceRequest = new WorkspaceRequest()
					{
						Name = workspaceName,
						Template = templateWorkspaceResponse,
						DownloadHandlerUrl = templateWorkspaceResponse.DownloadHandlerUrl,
						Matter = templateWorkspaceResponse.Matter.Value,
						ResourcePool = templateWorkspaceResponse.ResourcePool.Value,
						DefaultFileRepository = templateWorkspaceResponse.DefaultFileRepository.Value,
						DefaultCacheLocation = templateWorkspaceResponse.DefaultCacheLocation.Value,
						SqlServer = templateWorkspaceResponse.SqlServer.Value,
						SqlFullTextLanguage = templateWorkspaceResponse.SqlFullTextLanguage.ID,
						Status = templateWorkspaceResponse.Status

					};
					WorkspaceResponse workspaceResponse = await workspaceManager.CreateAsync(workspaceRequest);
					workspaceId = workspaceResponse.ArtifactID;
				}

				return workspaceId;
			}
			catch (Exception ex)
			{
				string errorMessage = $"Failed to create workspace in {nameof(CreateAsync)} for {nameof(workspaceName)} of {workspaceName} - {ex.Message}";
				Console.WriteLine(errorMessage);
				throw new TestHelpersException(errorMessage, ex);
			}
		}

		#endregion

		#region Delete

		/// <summary>
		/// Deletes a workspace for a given workspace artifact ID
		/// </summary>
		/// <param name="servicesMgr"></param>
		/// <param name="workspaceID"></param>
		/// <returns></returns>
		public static bool Delete(IServicesMgr servicesMgr, int workspaceID)
		{
			try
			{
				using (IWorkspaceManager workspaceManager = servicesMgr.CreateProxy<IWorkspaceManager>(ExecutionIdentity.CurrentUser))
				{
					workspaceManager.DeleteAsync(workspaceID).Wait();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("An error occurred deleting the Workspace: {0}", ex.Message);
				return false;
			}

			return true;
		}

		#endregion
	}
}
