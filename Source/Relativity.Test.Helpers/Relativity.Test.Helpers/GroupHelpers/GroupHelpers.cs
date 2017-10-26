using System;
using System.Linq;
using System.Threading.Tasks;
using Relativity.Services.Group;
using Relativity.Services.Permission;

namespace Relativity.Test.Helpers.GroupHelpers
{
	public static class GroupHelpers
	{
		public static object DeleteGroup { get; internal set; }

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
	}
}
