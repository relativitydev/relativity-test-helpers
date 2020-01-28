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
	public class FoldersHelper : IFoldersHelper
	{
		private readonly IHttpRequestHelper _httpRequestHelper;

		public FoldersHelper(IHttpRequestHelper httpRequestHelper)
		{
			_httpRequestHelper = httpRequestHelper;
		}

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
					var ResultSet = client.Repositories.Folder.Query(query);

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
				throw new TestHelpersException("Error Getting Folder Name", exception);
			}
		}

		public string GetFolderName(int folderArtifactId, int workspaceId)
		{
			try
			{
				const string routeName = "GetFolderNameAsync";

				var requestModel = new GetFolderNameRequestModel
				{
					FolderArtifactId = folderArtifactId,
					WorkspaceId = workspaceId
				};

				var responseString = _httpRequestHelper.SendPostRequest(requestModel, routeName);
				GetFolderNameResponseModel responseModel = JsonConvert.DeserializeObject<GetFolderNameResponseModel>(responseString);

				return responseModel.FolderName;
			}
			catch (Exception exception)
			{
				throw new TestHelpersException("Error Getting Folder Name", exception);
			}
		}
	}
}
