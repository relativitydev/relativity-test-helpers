using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.Services.Group;
using Relativity.Services.Interfaces.Shared;
using Relativity.Services.Interfaces.Shared.Models;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.Permission;
using QueryResult = Relativity.Services.Objects.DataContracts.QueryResult;

namespace Relativity.Test.Helpers.GroupHelpers
{
	public class GroupHelper
	{
		public static int CreateGroup(Services.ServiceProxy.ServiceFactory serviceFactory, String name)
		{
			int groupArtifactId;
			try
			{
				int clientId;
				using (IObjectManager objectManager = serviceFactory.CreateProxy<IObjectManager>())
				{
					QueryRequest clientQueryRequest = new QueryRequest
					{
						ObjectType = new ObjectTypeRef
						{
							ArtifactTypeID = 5
						}
					};
					QueryResult queryResult = objectManager.QueryAsync(-1, clientQueryRequest, 0, 10).ConfigureAwait(false).GetAwaiter().GetResult();
					if (queryResult.TotalCount == 0)
					{
						throw new Exception("Failed to query for Client.");
					}

					clientId = queryResult.Objects.First().ArtifactID;
				}

				using (Services.Interfaces.Group.IGroupManager groupManager =
					serviceFactory.CreateProxy<Services.Interfaces.Group.IGroupManager>())
				{
					Services.Interfaces.Group.Models.GroupRequest request = new Services.Interfaces.Group.Models.GroupRequest
					{
						Name = name,
						Client = new Securable<ObjectIdentifier>
						{
							Value = new ObjectIdentifier
							{
								ArtifactID = clientId
							}
						}
					};
					Services.Interfaces.Group.Models.GroupResponse response =
						groupManager.CreateAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
					groupArtifactId = response.ArtifactID;
				}

				return groupArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("Error creating group.", ex);
			}
		}

		public static bool DeleteGroup(Services.ServiceProxy.ServiceFactory serviceFactory, int artifactId)
		{
			try
			{
				using (Services.Interfaces.Group.IGroupManager groupManager = serviceFactory.CreateProxy<Services.Interfaces.Group.IGroupManager>())
				{
					groupManager.DeleteAsync(artifactId).Wait();
				}

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Group deletion threw an exception" + ex);
				return false;
			}
		}

		public static bool AddGroupToWorkspace(IPermissionManager permissionManager, Int32 eddsWorkspaceArtifactID, kCura.Relativity.Client.DTOs.Group group)
		{
			bool success = false;

			GroupSelector groupSelector = permissionManager.GetWorkspaceGroupSelectorAsync(eddsWorkspaceArtifactID).Result;
			GroupRef groupRef = groupSelector.DisabledGroups.FirstOrDefault(x => x.Name == group.Name);
			if (groupRef != null)
			{
				GroupSelector modifyGroupSelector = new GroupSelector() { LastModified = groupSelector.LastModified };
				modifyGroupSelector.EnabledGroups.Add(groupRef);
				Task task = permissionManager.AddRemoveWorkspaceGroupsAsync(eddsWorkspaceArtifactID, modifyGroupSelector);
				task.ConfigureAwait(false);
				task.Wait();
				success = true;
			}

			return success;
		}

		public static bool RemoveGroupFromWorkspace(IPermissionManager permissionManager, Int32 eddsWorkspaceArtifactID, kCura.Relativity.Client.DTOs.Group group)
		{
			bool success = false;

			GroupSelector groupSelector = permissionManager.GetWorkspaceGroupSelectorAsync(eddsWorkspaceArtifactID).Result;
			GroupRef groupRef = groupSelector.EnabledGroups.FirstOrDefault(x => x.Name == group.Name);
			if (groupRef != null)
			{
				GroupSelector modifyGroupSelector = new GroupSelector() { LastModified = groupSelector.LastModified };
				modifyGroupSelector.DisabledGroups.Add(groupRef);
				Task task = permissionManager.AddRemoveWorkspaceGroupsAsync(eddsWorkspaceArtifactID, modifyGroupSelector);
				task.ConfigureAwait(false);
				task.Wait();
				success = true;
			}

			return success;
		}

		private static async Task SecureItemFromGroupAsync(IPermissionManager mgr, int workspaceId, string groupName, int artifactId)
		{
			var itemSecurity = await mgr.GetItemLevelSecurityAsync(workspaceId, artifactId);
			if (!itemSecurity.Enabled)
			{
				itemSecurity.Enabled = true;
				await mgr.SetItemLevelSecurityAsync(workspaceId, itemSecurity);
			}
			var selection = await mgr.GetItemGroupSelectorAsync(workspaceId, artifactId);
			foreach (var enabledGroup in selection.EnabledGroups)
			{
				if (enabledGroup.Name == groupName)
				{
					selection.DisabledGroups.Add(enabledGroup);
					selection.EnabledGroups.Add(enabledGroup);
					await mgr.AddRemoveItemGroupsAsync(workspaceId, artifactId, selection);
					break;
				}
			}
		}

		/// <summary>
		/// Secures an artifact in a workspace from a group that is specified.
		/// The group and workspace must be setup before hand
		/// </summary>
		/// <param name="mgr"></param>
		/// <param name="workspaceId">The id of the workspace that contains the group and artifact that should be secured</param>
		/// <param name="groupName">The group that you want to artifact secured from</param>
		/// <param name="artifactId">The id of the artifact that should be secured</param>
		public static void SecureItemFromGroup(IPermissionManager mgr, int workspaceId, string groupName, int artifactId)
		{
			Task.WaitAll(SecureItemFromGroupAsync(mgr, workspaceId, groupName, artifactId));
		}

		/// <summary>
		/// Secures artifacts in a workspace from a group that is specified.
		/// The group and workspace must be setup before hand
		/// </summary>
		/// <param name="mgr"></param>
		/// <param name="workspaceId">The id of the workspace that contains the group and artifact that should be secured</param>
		/// <param name="groupName">The group that you want to artifact secured from</param>
		/// <param name="artifactIds">The ids of the artifacts that should be secured</param>
		public static void SecureItemFromGroup(IPermissionManager mgr, int workspaceId, string groupName, IEnumerable<int> artifactIds)
		{
			var tasks = new List<Task>();
			foreach (var artifactId in artifactIds)
			{
				tasks.Add(SecureItemFromGroupAsync(mgr, workspaceId, groupName, artifactId));
			}
			Task.WaitAll(tasks.ToArray());
		}
	}
}
