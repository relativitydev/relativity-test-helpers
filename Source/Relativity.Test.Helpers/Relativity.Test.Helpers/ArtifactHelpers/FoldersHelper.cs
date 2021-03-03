using Newtonsoft.Json;
using Relativity.API;
using Relativity.Services;
using Relativity.Services.Folder;
using Relativity.Test.Helpers.ArtifactHelpers.Interfaces;
using Relativity.Test.Helpers.Exceptions;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Linq;
using TestHelpersKepler.Interfaces.TestHelpersModule.v1.Models;
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
		private static bool? _keplerCompatible;

		#region Public Methods

		public static string GetFolderName(int folderArtifactId, IDBContext workspaceDbContext)
		{
			var keplerHelper = new KeplerHelper();

			if (keplerHelper.ForceDbContext()) return GetFolderNameWithDbContext(folderArtifactId, workspaceDbContext);

			if (_keplerCompatible == null)
			{
				_keplerCompatible = keplerHelper.IsVersionKeplerCompatibleAsync().ConfigureAwait(false).GetAwaiter().GetResult();
			}

			if (!_keplerCompatible.Value) return GetFolderNameWithDbContext(folderArtifactId, workspaceDbContext);

			var workspaceId = keplerHelper.GetWorkspaceIdFromDbContext(workspaceDbContext);
			return GetFolderName(folderArtifactId, workspaceId, keplerHelper);
		}

		public static int GetRootFolderArtifactID(int workspaceID, IServicesMgr svgMgr, string userName, string password)
		{
			try
			{
				using (IFolderManager folderManager = svgMgr.GetProxy<IFolderManager>(userName, password))
				{
					Services.Query query = new Services.Query();
					int length = 5;
					Condition queryCondition = new TextCondition("Name", Services.TextConditionEnum.EqualTo, WorkspaceHelpers.WorkspaceHelpers.GetWorkspaceName(svgMgr, workspaceID));
					string queryString = queryCondition.ToQueryString();
					query.Condition = queryString;

					FolderResultSet result = folderManager.QueryAsync(workspaceID, query, length).ConfigureAwait(false).GetAwaiter().GetResult();

					if (!result.Success)
					{
						throw new TestHelpersException("Folder was not found");
					}
					else if (result.TotalCount == 0)
					{
						throw new TestHelpersException("folder count was 0, so the folder was not found");
					}
					return result.Results.FirstOrDefault().Artifact.ArtifactID;
				}
			}
			catch (Exception exception)
			{
				throw new TestHelpersException($"Error Getting Root Folder ArtifactID [{nameof(workspaceID)}:{workspaceID}]", exception);
			}
		}

		public static int CreateFolder(IServicesMgr svcMgr, int workspaceId, string folderName)
		{
			try
			{
				var folder = new Relativity.Services.Folder.Folder
				{
					Name = folderName
				};

				int folderId;
				using (IFolderManager folderManager = svcMgr.GetProxy<IFolderManager>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD))
				{
					folderId = folderManager.CreateSingleAsync(workspaceId, folder).ConfigureAwait(false).GetAwaiter().GetResult();
				}

				return folderId;
			}
			catch (Exception ex)
			{
				throw new TestHelpersException($"Error creating folder [{nameof(folderName)}:{folderName}]", ex);
			}

		}

		#endregion


		#region DbContext Methods
		private static String GetFolderNameWithDbContext(Int32 folderArtifactID, IDBContext workspaceDbContext)
		{
			string sql = String.Format("select Name from eddsdbo.folder where ArtifactID = {0}", folderArtifactID);

			string folderName = workspaceDbContext.ExecuteSqlStatementAsScalar(sql).ToString();

			return folderName;
		}
		#endregion

		#region Kepler Methods
		public static string GetFolderName(int folderArtifactId, int workspaceId, KeplerHelper keplerHelper)
		{
			try
			{
				keplerHelper.UploadKeplerFiles();

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
	}
}
