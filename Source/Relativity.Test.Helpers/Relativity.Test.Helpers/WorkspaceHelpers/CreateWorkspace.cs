using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using IServicesMgr = Relativity.Test.Helpers.Interface.IServicesMgr;

namespace Relativity.Test.Helpers.WorkspaceHelpers
{

	/// <summary>
	/// 
	/// CreateWorkspaceAsync assists in creating Workspaces for tests
	/// 
	/// </summary>

	public class CreateWorkspace
	{

		public async static Task<Int32> CreateWorkspaceAsync(string workspaceName, string templeteName, IServicesMgr svcMgr, string userName, string password)
		{
			using (var client = svcMgr.GetProxy<IRSAPIClient>(userName, password))
			{
				client.APIOptions.WorkspaceID = -1;
				return await Create(client, workspaceName, templeteName);
			}
		}


		public static async Task<Int32> Create(IRSAPIClient proxy, string workspaceName, string templateName)
		{
			try
			{
				int workspaceID = 0;

				//Set the workspace ID
				proxy.APIOptions.WorkspaceID = -1;
				var resultSet = GetArtifactIdOfTemplate(proxy, templateName);
				
				if (resultSet.Success)
				{
					//Save the artifact ID of the template workspace
					int templateArtifactID = resultSet.Results.FirstOrDefault().Artifact.ArtifactID;

					//Create the workspace DTO
					var workspaceDTO = new kCura.Relativity.Client.DTOs.Workspace();

					//Set primary fields
					//The name of the sample data is being set to a random string so that sample data can be debugged
					//and never causes collisions. You can set this to any string that you want
					workspaceDTO.Name = workspaceName;

					//Get the server id or use the configuration value
					//NOTE: We're using the server ID from the template workspace. This may not be correct and may need to be updatedServerID is hard-coded since we don't have a way to get the server ID
					int? serverID = resultSet.Results.FirstOrDefault().Artifact.ServerID;
					if (ConfigurationManager.AppSettings.AllKeys.Contains("ServerID"))
					{
						int serverIDConfigValue = Convert.ToInt32(ConfigurationManager.AppSettings["ServerID"]);
						if (serverIDConfigValue != serverID)
						{
							serverID = serverIDConfigValue;
						}
					}
					workspaceDTO.ServerID = serverID.Value;

					kCura.Relativity.Client.ProcessOperationResult result = new kCura.Relativity.Client.ProcessOperationResult();
					try
					{
						//Create the workspace
						result = proxy.Repositories.Workspace.CreateAsync(templateArtifactID, workspaceDTO);
						if (result.Success)
						{
							//Manually check the results and return the workspace ID synchronously
							kCura.Relativity.Client.ProcessInformation info = proxy.GetProcessState(proxy.APIOptions, result.ProcessID);
							int iteration = 0;
							while (info.State != ProcessStateValue.Completed)
							{
								System.Threading.Thread.Sleep(10000);
								info = proxy.GetProcessState(proxy.APIOptions, result.ProcessID);

								if (iteration > 6)
								{
									Console.WriteLine("Workspace creation timed out");
								}
								iteration++;
							}

							workspaceID = (int) info.OperationArtifactIDs.FirstOrDefault();

							Console.WriteLine("Workspace Created with Artiafact ID :" + workspaceID);

							//DataHelper.DeleteData[Constants.WORKSPACE_ID_LIST].Add(workspaceID);

							return workspaceID;
						}
						else
						{
							throw new System.Exception(String.Format("workspace creation failed: {0}", result.Message));
						}
					}
					catch (Exception ex)
					{
						throw new System.Exception(String.Format("Unhandled Exception : {0}", ex));
					}
				}
				else
				{
					return workspaceID;
				}
			}
			catch (Exception ex)
			{
				throw new System.Exception("Create Workspace failed", ex);
			}
			}

		private static QueryResultSet<Workspace> GetArtifactIdOfTemplate(IRSAPIClient proxy, string templateName)
		{
			try
			{
				//Query for the starter template
				Query<Workspace> query = new Query<Workspace>();
				query.Condition = new kCura.Relativity.Client.TextCondition(FieldFieldNames.Name, TextConditionEnum.EqualTo, templateName);

				query.Fields = kCura.Relativity.Client.DTOs.FieldValue.AllFields;
				kCura.Relativity.Client.DTOs.QueryResultSet<kCura.Relativity.Client.DTOs.Workspace> resultSet = proxy.Repositories.Workspace.Query(query, 0);
				return resultSet;

			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}
