using System;
using System.Linq;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using kCura.Vendor.Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Relativity.API;
//using IServicesMgr = Relativity.Test.Helpers.Interface.IServicesMgr;
using IServicesMgr = Relativity.API.IServicesMgr;
using Relativity.Test.Helpers.ServiceFactory.Extentions;

namespace Relativity.Test.Helpers.ArtifactHelpers
{

	/// <summary>
	/// 
	/// Helpers to interact with Folders in Relativity
	/// 
	/// </summary>
	/// 
	public class Folders
	{
		public static Int32 GetRootFolderArtifactID(Int32 workspaceID, IServicesMgr svgMgr, string userName, string password)
		{
			using (IRSAPIClient client = svgMgr.GetProxy<IRSAPIClient>(userName, password))
			{
				Query<Folder> query = new Query<Folder>();
				query.Condition = new TextCondition(FolderFieldNames.Name, TextConditionEnum.EqualTo,WorkspaceHelpers.WorkspaceHelpers.GetWorkspaceName(client, workspaceID));
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


		public static String GetFolderName(Int32 folderArtifactID, IDBContext workspaceDbContext)
		{
			string sql = String.Format("select Name from folder where ArtifactID = {0}", folderArtifactID);

			string folderName = workspaceDbContext.ExecuteSqlStatementAsScalar(sql).ToString();

			return folderName;
		}

	}
}
