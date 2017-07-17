using System;
using System.Collections.Generic;
using kCura.Relativity.Client;
using System.Threading;
using kCura.Relativity.Client.DTOs;
using System.Linq;

namespace Relativity.Test.Helpers.Application
{
	public class ApplicationHelpers
	{
		public static Int32 ImportApplication(IRSAPIClient client, Int32 workspaceId, bool forceFlag, string filePath, string applicationName, int appArtifactID = -1)
		{
			Console.WriteLine("Starting Import Application.....");
			int artifactID = 0;
			client.APIOptions.WorkspaceID = workspaceId; //set the target workspace of application to be imported.

			// Create an application install request. 
			// This list contains the ArtifactID for each Relativity Application that you want to install.
			List<int> appsToOverride = new List<int>();

			// Set the forceFlag to true. The forceFlag unlocks any applications in the workspace 
			// that conflict with the application that you are loading. The applications must be unlocked 
			// for the install operation to succeed.

			AppInstallRequest appInstallRequest = new AppInstallRequest();

			appInstallRequest.FullFilePath = filePath;
			appInstallRequest.ForceFlag = forceFlag;
			appInstallRequest.AppsToOverride.Add(appArtifactID);

			try
			{
				ProcessOperationResult por = null;
				por = client.InstallApplication(client.APIOptions, appInstallRequest);

				if (por.Success)
				{
					while (client.GetProcessState(client.APIOptions, por.ProcessID).State == ProcessStateValue.Running)
					{
						Thread.Sleep(10);
					}

					client.GetProcessState(client.APIOptions, por.ProcessID);
					Console.WriteLine("Import Application Application complete.....");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to import Application" + ex.Message);
			}

			Console.WriteLine("Querying for Application artifact id....");
			kCura.Relativity.Client.DTOs.Query<kCura.Relativity.Client.DTOs.RelativityApplication> query = new kCura.Relativity.Client.DTOs.Query<kCura.Relativity.Client.DTOs.RelativityApplication>();
			query.Fields.Add(new FieldValue(RelativityApplicationFieldNames.Name));
			query.Condition = new kCura.Relativity.Client.TextCondition(RelativityApplicationFieldNames.Name , kCura.Relativity.Client.TextConditionEnum.EqualTo, applicationName);
			kCura.Relativity.Client.DTOs.QueryResultSet<kCura.Relativity.Client.DTOs.RelativityApplication> queryResultSet = client.Repositories.RelativityApplication.Query(query);

			if (queryResultSet != null)
			{
				artifactID = queryResultSet.Results.FirstOrDefault().Artifact.ArtifactID;
				Console.WriteLine("Application artifactid is " + artifactID);
			}

			Console.WriteLine("Exiting Import Application method.....");
			return artifactID;
		}
	}
}
