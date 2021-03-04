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

		public static string GetFolderName(IServicesMgr svcMgr, int folderArtifactId, int workspaceId)
		{
			Query query = new Services.Query();
			query.Condition = $"'ArtifactID' == {folderArtifactId}";
			using (IFolderManager folderManager =
				svcMgr.GetProxy<IFolderManager>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD))
			{
				FolderResultSet result = folderManager.QueryAsync(workspaceId, query, 100).ConfigureAwait(false).GetAwaiter().GetResult();
				if (!result.Success)
				{
					throw new Exception("Error Querying for Folder");
				}

				string folderName = result.Results.First().Artifact.Name;
				return folderName;
			}
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

	}
}
