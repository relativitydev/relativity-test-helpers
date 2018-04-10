using Relativity.Services.Group;
using Relativity.Services.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
