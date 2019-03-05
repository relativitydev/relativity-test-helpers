using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using System;
using System.Linq;
using DTOs = kCura.Relativity.Client.DTOs;

namespace Relativity.Test.Helpers.Objects.Folder
{

	public class FolderHelper
	{
		private TestHelper _helper;

		public FolderHelper(TestHelper helper)
		{
			_helper = helper;
		}

		/// <summary>
		/// Returns the artifactID of the root folder by querying for the folder that matches the workspace name.
		/// </summary>
		/// <param name="workspaceID"></param>
		/// <param name="workspaceName"></param>
		/// <returns></returns>
		public Int32 GetRootFolderArtifactID(Int32 workspaceID, string workspaceName)
		{
			using (IRSAPIClient client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.CurrentUser))
			{
				Query<DTOs.Folder> query = new Query<DTOs.Folder>();
				query.Condition = new TextCondition(FolderFieldNames.Name, TextConditionEnum.EqualTo, workspaceName);
				query.Fields = FieldValue.NoFields;
				var ResultSet = client.Repositories.Folder.Query(query);

				if (!ResultSet.Success)
				{
					throw new System.Exception("Folder was not found");
				}
				else if (ResultSet.TotalCount == 0)
				{
					throw new System.Exception("folder count was 0, so the folder was not found");
				}
				return ResultSet.Results.FirstOrDefault().Artifact.ArtifactID;
			}
		}

		/// <summary>
		/// Returns folder name by artifact ID.
		/// </summary>
		/// <param name="workspaceID"></param>
		/// <param name="folderArtifactID"></param>
		/// <returns></returns>
		public String GetFolderName(int workspaceID, Int32 folderArtifactID)
		{
			string sql = String.Format("select Name from folder where ArtifactID = {0}", folderArtifactID);

			string folderName = _helper.GetDBContext(workspaceID).ExecuteSqlStatementAsScalar(sql).ToString();

			return folderName;
		}
	}
}