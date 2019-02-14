using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using DTOs = kCura.Relativity.Client.DTOs;
using Relativity.API;
using Relativity.Test.Helpers.Objects.Application.Exceptions;
using System;
using System.Linq;
using System.Threading;

namespace Relativity.Test.Helpers.Objects.Application
{
	public class ApplicationHelper
	{
		private TestHelper _helper;

		public ApplicationHelper(TestHelper helper)
		{
			_helper = helper;
		}
		public Int32 Import(Int32 workspaceId, bool forceFlag, string filePath, string appName, int appArtifactID = -1)
		{
			// Set the forceFlag to true. The forceFlag unlocks any applications in the workspace 
			// that conflict with the application that you are loading. The applications must be unlocked 
			// for the install operation to succeed.

			var appInstallRequest = new AppInstallRequest();

			appInstallRequest.FullFilePath = filePath;
			appInstallRequest.ForceFlag = forceFlag;
			appInstallRequest.AppsToOverride.Add(appArtifactID);

			ProcessOperationResult por;

			using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
			{
				client.APIOptions.WorkspaceID = workspaceId;
				Console.WriteLine("Starting Import Application.....");
				por = client.InstallApplication(client.APIOptions, appInstallRequest);
			}

			if (por.Success)
			{
				PollForInstallSuccess(por.ProcessID);
			}
			else
			{
				throw new ApplicationInstallException($"There was an error installing the application {por.Message}");
			}

			return RetrieveWorkspaceID(appName);
		}

		private void PollForInstallSuccess(Guid processID)
		{
			ProcessInformation state;
			do
			{
				Thread.Sleep(10);
				using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
				{
					state = client.GetProcessState(client.APIOptions, processID);
				}

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

		public Int32 RetrieveWorkspaceID(string applicationName)
		{
			int artifactID = 0;

			Console.WriteLine("Querying for Application artifact id....");
			var query = new Query<DTOs.RelativityApplication>();
			query.Fields.Add(new FieldValue(RelativityApplicationFieldNames.Name));
			query.Condition = new TextCondition(RelativityApplicationFieldNames.Name, TextConditionEnum.EqualTo, applicationName);
			QueryResultSet<DTOs.RelativityApplication> queryResultSet;

			using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System))
			{
				queryResultSet = client.Repositories.RelativityApplication.Query(query);
			}
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
