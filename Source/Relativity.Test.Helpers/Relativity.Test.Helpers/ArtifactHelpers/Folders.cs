using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Relativity.Test.Helpers.ArtifactHelpers.Interfaces;
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
	public class Folders : IFoldersHelper
	{
		
		#region DbContext Methods
		public static String GetFolderName(Int32 folderArtifactID, IDBContext workspaceDbContext)
		{
			string sql = String.Format("select Name from folder where ArtifactID = {0}", folderArtifactID);

			string folderName = workspaceDbContext.ExecuteSqlStatementAsScalar(sql).ToString();

			return folderName;
		}
		#endregion

		#region Kepler Methods
		public static string GetFolderName(int folderArtifactId, int workspaceId)
		{
			try
			{
				const string routeName = Constants.Kepler.RouteNames.GetFolderNameAsync;

				GetFolderNameRequestModel requestModel = new GetFolderNameRequestModel
				{
					FolderArtifactId = folderArtifactId,
					WorkspaceId = workspaceId
				};

				var httpRequestHelper = new HttpRequestHelper();
				string responseString = httpRequestHelper.SendPostRequest(requestModel, routeName);
				GetFolderNameResponseModel responseModel = JsonConvert.DeserializeObject<GetFolderNameResponseModel>(responseString);

				return responseModel.FolderName;
			}
			catch (Exception exception)
			{
				throw new TestHelpersException($"Error Getting Folder Name [{nameof(folderArtifactId)}:{folderArtifactId}]", exception);
			}
		}

		#endregion

		public static int GetRootFolderArtifactID(int workspaceID, IServicesMgr svgMgr, string userName, string password)
		{
			try
			{
				using (IRSAPIClient client = svgMgr.GetProxy<IRSAPIClient>(userName, password))
				{
					client.APIOptions.WorkspaceID = workspaceID;
					Query<Folder> query = new Query<Folder>();
					query.Condition = new TextCondition(FolderFieldNames.Name, TextConditionEnum.EqualTo, WorkspaceHelpers.WorkspaceHelpers.GetWorkspaceName(client, workspaceID));
					query.Fields = FieldValue.NoFields;
					QueryResultSet<Folder> ResultSet = client.Repositories.Folder.Query(query);

					if (!ResultSet.Success)
					{
						throw new TestHelpersException("Folder was not found");
					}
					else if (ResultSet.TotalCount == 0)
					{
						throw new TestHelpersException("folder count was 0, so the folder was not found");
					}
					return ResultSet.Results.FirstOrDefault().Artifact.ArtifactID;
				}
			}
			catch (Exception exception)
			{
				throw new TestHelpersException($"Error Getting Root Folder ArtifactID [{nameof(workspaceID)}:{workspaceID}]", exception);
			}
		}

	}
}
