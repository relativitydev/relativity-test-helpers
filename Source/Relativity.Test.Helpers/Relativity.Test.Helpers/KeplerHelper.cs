using kCura.Relativity.Client;
using Relativity.API;
using Relativity.Services.Interfaces.LibraryApplication;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.Exceptions;
using Relativity.Test.Helpers.Kepler;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers
{
	public class KeplerHelper
	{
		public bool ForceDbContext()
		{
			return ConfigurationHelper.FORCE_DBCONTEXT.Trim().ToLower().Equals("true");
		}

		public int GetWorkspaceIdFromDbContext(IDBContext dbContext)
		{
			SqlConnection connection = dbContext.GetConnection();
			var connectionString = connection.ConnectionString;

			var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
			var initialCatalog = connectionStringBuilder.InitialCatalog;

			var workspaceIdString = initialCatalog.Remove(0, 4);

			int workspaceId;

			if (workspaceIdString.Equals(""))
			{
				workspaceId = -1;
				return workspaceId;
			}
			int.TryParse(workspaceIdString, out workspaceId);

			return workspaceId;
		}

		#region Kepler Compatibility Check

		public async Task<bool> IsVersionKeplerCompatibleAsync()
		{
			try
			{
				string currentInstanceRelativity = await GetInstanceRelativityVersionAsync();

				Version minVersion = new Version(Constants.Kepler.MINIMUM_KEPLER_COMPATIBLE_VERSION);
				Version currentInstanceVersion = new Version(currentInstanceRelativity);

				int result = currentInstanceVersion.CompareTo(minVersion);

				return result >= 0;
			}
			catch (Exception ex)
			{
				string exception = $"An error occurred in ({nameof(IsVersionKeplerCompatibleAsync)}) : {ex.Message}";
				Console.WriteLine(exception);
				throw new TestHelpersApplicationInstallException(exception, ex);
			}
		}

		private async Task<string> GetInstanceRelativityVersionAsync()
		{
			try
			{
				var httpMessageHandler = new HttpClientHandler()
				{
					AllowAutoRedirect = false,
					UseDefaultCredentials = true
				};
				HttpClient httpClient = new HttpClient(httpMessageHandler)
				{
					BaseAddress =
						new Uri(
							$"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.REST_SERVER_ADDRESS}/Relativity")
				};

				string encoded = System.Convert.ToBase64String(
					Encoding.ASCII.GetBytes(ConfigurationHelper.ADMIN_USERNAME + ":" + ConfigurationHelper.DEFAULT_PASSWORD));
				httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", string.Empty);
				httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {encoded}");

				StringContent content = new StringContent("");
				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

				string url =
					"/relativity.rest/api/Relativity.Services.InstanceDetails.IInstanceDetailsModule/InstanceDetailsService/GetRelativityVersionAsync";
				HttpResponseMessage httpResponse = await httpClient.PostAsync(url, content);
				string instanceRelativityVersion = await httpResponse.Content.ReadAsStringAsync();
				instanceRelativityVersion = instanceRelativityVersion.Replace("\"", "");
				return instanceRelativityVersion;
			}
			catch (Exception ex)
			{
				string exception = $"An error occurred in ({nameof(GetInstanceRelativityVersionAsync)}) : {ex.Message}";
				Console.WriteLine(exception);
				throw new TestHelpersApplicationInstallException(exception, ex);
			}
		}

		#endregion

		#region Upload Kepler Files

		public void UploadKeplerFiles()
		{
			var fileLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			InstallKeplerRap();

			var keplerDlls = new List<string>
			{
				Constants.Kepler.INTERFACES_DLL_NAME,
				Constants.Kepler.SERVICES_DLL_NAME
			};
			foreach (var file in keplerDlls)
			{
				InstallKeplerResourceFile(file, fileLocation);
			}
		}

		private int InstallKeplerRap()
		{
			try
			{
				IApplicationInstallManager applicationInstallManager = GetServiceFactory().CreateProxy<IApplicationInstallManager>();
				ILibraryApplicationManager libraryApplicationManager = GetServiceFactory().CreateProxy<ILibraryApplicationManager>();
				IRSAPIClient rsapiClient = GetServiceFactory().CreateProxy<IRSAPIClient>();

				ApplicationInstallHelper applicationInstallHelper = new ApplicationInstallHelper(
					applicationInstallManager: applicationInstallManager,
					libraryApplicationManager: libraryApplicationManager, protocol: ConfigurationHelper.SERVER_BINDING_TYPE,
					serverAddress: ConfigurationHelper.REST_SERVER_ADDRESS, username: ConfigurationHelper.ADMIN_USERNAME,
					password: ConfigurationHelper.DEFAULT_PASSWORD);

				string keplerRapFileParentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

				if (keplerRapFileParentFolder == null)
				{
					throw new TestHelpersException("Bin folder path string is empty");
				}

				DirectoryInfo keplerRapFileParentFolderDirectoryInfo = new DirectoryInfo(keplerRapFileParentFolder);
				if (!keplerRapFileParentFolderDirectoryInfo.Exists)
				{
					throw new TestHelpersException($"{nameof(keplerRapFileParentFolder)} directory doesn't exist");
				}

				const string keplerRapFileNameWithExtension = Constants.Kepler.KeplerTestRap.KEPLER_TEST_APP_NAME + ".rap";

				string keplerRapFullFilePath = Path.Combine(keplerRapFileParentFolder, keplerRapFileNameWithExtension);
				FileInfo keplerRapFilePathFileInfo = new FileInfo(keplerRapFullFilePath);
				if (!keplerRapFilePathFileInfo.Exists)
				{
					throw new TestHelpersException($"RAP File ({nameof(keplerRapFullFilePath)}) doesn't exist");
				}

				FileStream fileStream = File.OpenRead(keplerRapFullFilePath);

				int keplerTestRapArtifactId = applicationInstallHelper.InstallApplicationAsync(
					applicationName: Constants.Kepler.KeplerTestRap.KEPLER_TEST_APP_NAME,
					fileStream: fileStream,
					workspaceId: -1,
					unlockApps: true).ConfigureAwait(false).GetAwaiter().GetResult();

				return keplerTestRapArtifactId;
			}
			catch (Exception ex)
			{
				throw new TestHelpersException($"{nameof(InstallKeplerRap)} - Application installation failed for RAP {Constants.Kepler.KeplerTestRap.KEPLER_TEST_APP_NAME} - Exception: {ex.Message}");
			}
		}

		private bool InstallKeplerResourceFile(string keplerDll, string fileLocation)
		{
			using (IRSAPIClient rsapiClient = GetServiceFactory().CreateProxy<IRSAPIClient>())
			{
				try
				{
					var rfRequest = new ResourceFileRequest
					{
						AppGuid = new Guid(Constants.Kepler.KeplerTestRap.KEPLER_TEST_APP_GUID),
						FullFilePath = Path.Combine(fileLocation, keplerDll),
						FileName = keplerDll
					};

					var results = rsapiClient.PushResourceFiles(rsapiClient.APIOptions, new List<ResourceFileRequest> { rfRequest });

					//sleep for 20 seconds to allow kepler to initialize
					System.Threading.Thread.Sleep(Constants.Kepler.TWENTY_SECONDS);

					return results.Success;
				}
				catch (Exception ex)
				{
					throw new TestHelpersException($"{nameof(InstallKeplerResourceFile)} - Could not upload ({keplerDll}) - Exception: {ex.Message}");
				}
			}
		}

		#endregion

		#region Get Service Factory
		private Services.ServiceProxy.ServiceFactory GetServiceFactory()
		{
			var relativityServicesUri = new Uri($"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.RELATIVITY_INSTANCE_ADDRESS}/Relativity.Services");
			var relativityRestUri = new Uri($"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.REST_SERVER_ADDRESS.ToLower().Replace("-services", "")}/Relativity.Rest/Api");

			Relativity.Services.ServiceProxy.UsernamePasswordCredentials usernamePasswordCredentials = new Relativity.Services.ServiceProxy.UsernamePasswordCredentials(
				username: ConfigurationHelper.ADMIN_USERNAME,
				password: ConfigurationHelper.DEFAULT_PASSWORD);

			ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(
				relativityServicesUri: relativityServicesUri,
				relativityRestUri: relativityRestUri,
				credentials: usernamePasswordCredentials);

			var serviceFactory = new Services.ServiceProxy.ServiceFactory(
				settings: serviceFactorySettings);

			return serviceFactory;
		}

		#endregion
	}
}
