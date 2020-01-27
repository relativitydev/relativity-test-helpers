using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Relativity.Test.Helpers.Exceptions;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models;
//using IServicesMgr = Relativity.Test.Helpers.Interface.IServicesMgr;
using IServicesMgr = Relativity.API.IServicesMgr;

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
				query.Condition = new TextCondition(FolderFieldNames.Name, TextConditionEnum.EqualTo, WorkspaceHelpers.WorkspaceHelpers.GetWorkspaceName(client, workspaceID));
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


		//public static String GetFolderName(Int32 folderArtifactID, IDBContext workspaceDbContext)
		//{
		//	string sql = String.Format("select Name from folder where ArtifactID = {0}", folderArtifactID);

		//	string folderName = workspaceDbContext.ExecuteSqlStatementAsScalar(sql).ToString();

		//	return folderName;
		//}

		public static string GetFolderName(int folderArtifactId, int workspaceId)
		{
			const string routeName = "GetFolderName";

			HttpRequestHelper<GetFolderNameRequestModel> httpRequestHelper = new HttpRequestHelper<GetFolderNameRequestModel>();

			var requestModel = new GetFolderNameRequestModel
			{
				FolderArtifactId = folderArtifactId,
				WorkspaceId = workspaceId
			};

			StringContent content = httpRequestHelper.GetRequestContent(requestModel);
			string restAddress = httpRequestHelper.GetRestAddress(routeName);

			GetFolderNameResponseModel responseModel;
			HttpClient client = httpRequestHelper.GetClient();
			using (client)
			{
				var response = client.PostAsync(restAddress, content).Result;
				if (!response.IsSuccessStatusCode)
				{
					throw new TestHelpersException("Failed to Get Folder Name.");
				}
				string responseString = response.Content.ReadAsStringAsync().Result;
				responseModel = JsonConvert.DeserializeObject<GetFolderNameResponseModel>(responseString);
			}

			return responseModel.FolderName;
		}
	}
}
