using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using System;
using System.Linq;
using System.Threading;

namespace Relativity.Test.Helpers.Application
{
	public class ApplicationHelpers
	{
		public static void ImportApplication(IRSAPIClient client, Int32 workspaceId, bool forceFlag, string filePath, int appArtifactID = -1)
		{
			Console.WriteLine("Starting Import Application.....");
			client.APIOptions.WorkspaceID = workspaceId; //set the target workspace of application to be imported.

			// Set the forceFlag to true. The forceFlag unlocks any applications in the workspace 
			// that conflict with the application that you are loading. The applications must be unlocked 
			// for the install operation to succeed.

			var appInstallRequest = new AppInstallRequest();

			appInstallRequest.FullFilePath = filePath;
			appInstallRequest.ForceFlag = forceFlag;
			appInstallRequest.AppsToOverride.Add(appArtifactID);

			var por = client.InstallApplication(client.APIOptions, appInstallRequest);

			if (por.Success)
			{
				ProcessInformation state;
				do
				{
					Thread.Sleep(10);
					state = client.GetProcessState(client.APIOptions, por.ProcessID);

				} while (state.State == ProcessStateValue.Running);

				if (state.State == ProcessStateValue.CompletedWithError)
				{
					throw new ApplicationInstallException(state.Message ?? state.Status ?? "The install completed an unknown error");
				}
				else if (state.State == ProcessStateValue.HandledException || state.State == ProcessStateValue.UnhandledException)
				{
					throw new ApplicationInstallException(state.Message ?? state.Status ?? "The install failed with a unknown error");
				}
			}
			else
			{
				throw new ApplicationInstallException($"There was an error installing the application {por.Message}");
			}
		}

		public static Int32 ImportApplication(IRSAPIClient client, Int32 workspaceId, bool forceFlag, string filePath, string applicationName, int appArtifactID = -1)
		{
			int artifactID = 0;
			ImportApplication(client, workspaceId, forceFlag, filePath, appArtifactID);

			Console.WriteLine("Querying for Application artifact id....");
			var query = new Query<kCura.Relativity.Client.DTOs.RelativityApplication>();
			query.Fields.Add(new FieldValue(RelativityApplicationFieldNames.Name));
			query.Condition = new TextCondition(RelativityApplicationFieldNames.Name, TextConditionEnum.EqualTo, applicationName);
			var queryResultSet = client.Repositories.RelativityApplication.Query(query);

			if (queryResultSet != null)
			{
				var result = queryResultSet.Results.FirstOrDefault();
				if (result == null || result.Artifact == null)
				{
					throw new ApplicationInstallException($"Could not find application with name {applicationName}.");
				}
				artifactID = result.Artifact.ArtifactID;
				Console.WriteLine("Application artifactid is " + artifactID);
			}

			Console.WriteLine("Exiting Import Application method.....");
			return artifactID;
		}
	}
}
