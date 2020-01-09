using kCura.Relativity.Client;
using Relativity.Kepler.Transport;
using Relativity.Services.Interfaces.LibraryApplication;
using Relativity.Services.Interfaces.LibraryApplication.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.Kepler
{
	public class ApplicationInstallHelper : IApplicationInstallHelper, IDisposable
	{
		private readonly IApplicationInstallManager _applicationInstallManager;
		private readonly ILibraryApplicationManager _libraryApplicationManager;
		private readonly IRSAPIClient _rsapiClient;

		public ApplicationInstallHelper(IRSAPIClient rsapiClient, IApplicationInstallManager applicationInstallManager, ILibraryApplicationManager libraryApplicationManager)
		{
			_rsapiClient = rsapiClient ?? throw new ArgumentNullException($"Parameter ({nameof(rsapiClient)}) cannot be null");
			_applicationInstallManager = applicationInstallManager ?? throw new ArgumentNullException($"Parameter ({nameof(applicationInstallManager)}) cannot be null");
			_libraryApplicationManager = libraryApplicationManager ?? throw new ArgumentNullException($"Parameter ({nameof(libraryApplicationManager)}) cannot be null");
		}

		public void Dispose()
		{
			_rsapiClient.Dispose();
			_applicationInstallManager.Dispose();
			_libraryApplicationManager.Dispose();
		}

		public async Task<int> InstallApplicationAsync(string applicationName, string rapFilePath, int workspaceId, bool unlockApps)
		{
			try
			{
				List<int> workspaces = new List<int> { workspaceId };
				int workspaceApplicationInstallId;
				string rapFileName = Path.GetFileName(rapFilePath);

				if (await IsVersionKeplerCompatibleAsync())
				{
					InstallApplicationRequest request = new InstallApplicationRequest
					{
						WorkspaceIDs = workspaces,
						UnlockApplications = unlockApps
					};

					int libraryApplicationInstallId;

					if (!await DoesApplicationExistAsync(-1, rapFileName))
					{
						libraryApplicationInstallId = await CreateLibraryApplicationAsync(rapFilePath);
					}
					else
					{
						libraryApplicationInstallId = await GetLibraryApplicationIdAsync(applicationName);
					}

					InstallApplicationResponse response = await _applicationInstallManager.InstallApplicationAsync(-1, libraryApplicationInstallId, request);

					if (response.Results.Count == workspaces.Count)
					{
						string info = string.Format($"Queuing {response.Results.Count} installation(s) for the Library Application with ArtifactID {libraryApplicationInstallId}.");
						Console.WriteLine(info);
					}
					else
					{
						string info = string.Format($"Queuing {response.Results.Count} installation(s) for the Library Application with ArtifactID {libraryApplicationInstallId} " +
																				$"since one or more workspaces already have the same version of this application installed.");
						Console.WriteLine(info);
					}

					workspaceApplicationInstallId = response.Results.First().ApplicationInstallID;

					// The following function will poll for the installation status until the installation reaches a terminal state.
					InstallStatusCode status = PollForTerminalStatusAsync(async () => await _libraryApplicationManager.GetLibraryInstallStatusAsync(-1, libraryApplicationInstallId)).Result;
					Console.WriteLine($@"Library Installation has terminated with the following status: {status}.");

					status = PollForTerminalStatusAsync(async () => await _applicationInstallManager.GetStatusAsync(-1, libraryApplicationInstallId, workspaceApplicationInstallId)).Result;
					Console.WriteLine($@"Workspace Installation has terminated with the following status: {status}.");
				}
				else
				{
					// TODO: use the old RSAPI method here
					workspaceApplicationInstallId = 0;
				}

				return workspaceApplicationInstallId;
			}
			catch (Exception ex)
			{
				string exception = $"An error occurred: {ex.Message}";
				Console.WriteLine(exception);
				throw;
			}
		}

		private async Task<int> CreateLibraryApplicationAsync(string rapFilePath)
		{
			try
			{
				using (Stream stream = File.OpenRead(rapFilePath))
				{
					CreateLibraryApplicationResponse response = await _libraryApplicationManager.CreateAsync(-1, new KeplerStream(stream));
					string info = string.Format($"The file located at {rapFilePath} is uploading to the application library.");
					Console.WriteLine(info);

					return response.ApplicationInstallID;
				}
			}
			catch (Exception ex)
			{
				string exception = $"An error occurred: {ex.Message}";
				Console.WriteLine(exception);
				throw;
			}
		}

		public async Task<bool> DoesApplicationExistAsync(int workspaceId, string rapFileName)
		{
			try
			{
				List<LibraryApplicationResponse> allApps;
				bool result;

				if (workspaceId != -1)
				{
					allApps = await _libraryApplicationManager.ReadAllAsync(workspaceId);

					if (allApps.Exists(x => x.FileName.Equals(rapFileName)))
					{
						LibraryApplicationResponse app = allApps.Find(x => x.FileName.Equals(rapFileName));

						GetInstallStatusResponse appStatus = await _applicationInstallManager.GetStatusAsync(workspaceId, app.Guids.First(), app.ArtifactID);
						result = appStatus.InstallStatus.Code == InstallStatusCode.Completed;
					}
					else
					{
						result = false;
					}
				}
				else
				{
					allApps = await _libraryApplicationManager.ReadAllAsync(workspaceId);
					result = allApps.Exists(x => x.FileName.Equals(rapFileName));
				}

				return result;
			}
			catch (Exception ex)
			{
				string exception = $"An error occurred in ({nameof(DoesApplicationExistAsync)}) | workspaceId: ({workspaceId}) | rapFileName: ({rapFileName}) : {ex.Message}";
				Console.WriteLine(exception);
				throw;
			}
		}

		private async Task<int> GetLibraryApplicationIdAsync(string applicationName)
		{
			return await Task.FromResult(0);
		}

		private async Task<bool> IsVersionKeplerCompatibleAsync()
		{
			// TODO: Use this address to see which version of relativity we are at
			//  relativity.rest/api/Relativity.Services.InstanceDetails.IInstanceDetailsModule/InstanceDetailsService/GetRelativityVersionAsync
			// https://stackoverflow.com/questions/7568147/compare-version-numbers-without-using-split-function

			return await Task.FromResult(true);
		}



		public async Task<InstallStatusCode> PollForTerminalStatusAsync(Func<Task<GetInstallStatusResponse>> func)
		{
			GetInstallStatusResponse result;
			Stopwatch watch = new Stopwatch();
			int refreshInterval = 5;
			bool continuePolling = true;
			watch.Start();

			do
			{
				if (watch.Elapsed.TotalMinutes > 2)
				{
					throw new System.Exception($"The terminal status for this application installation could not be obtained. " +
																		 $"Total polling time exceeded timeout value of {2} minutes.");
				}

				result = await func.Invoke();

				switch (result.InstallStatus.Code)
				{
					case InstallStatusCode.Pending:
					case InstallStatusCode.InProgress:
						Console.WriteLine(@"Application installation is pending or in progress.");
						break;

					default:
						continuePolling = false;
						break;
				}

				if (continuePolling)
				{
					await Task.Delay(TimeSpan.FromSeconds(refreshInterval));
				}

			} while (continuePolling);

			watch.Stop();
			return result.InstallStatus.Code;
		}


	}
}
