using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using OutsideIn.Options;
using Relativity.API;
using Relativity.Services.Interfaces.LibraryApplication;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.Exceptions;
using Relativity.Test.Helpers.Kepler;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace Relativity.Test.Helpers
{
	public class KeplerHelper
	{
		private readonly string _fileLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		//private readonly string _fileLocation = @"S:\temp_kepler_files";

		public bool ForceDbContext()
		{
			return ConfigurationHelper.FORCE_DBCONTEXT.Trim().ToLower().Equals("true");
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
							$"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.RSAPI_SERVER_ADDRESS}/Relativity")
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
			//var fileLocation = @"S:\temp_kepler_files";

			InstallKeplerTestRap(fileLocation);

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

		private int InstallKeplerTestRap(string fileLocation)
		{
			try
			{
				IApplicationInstallManager applicationInstallManager =
					GetServiceFactory().CreateProxy<IApplicationInstallManager>();
				ILibraryApplicationManager libraryApplicationManager =
					GetServiceFactory().CreateProxy<ILibraryApplicationManager>();
				IRSAPIClient rsapiClient = GetServiceFactory().CreateProxy<IRSAPIClient>();

				var applicationInstallHelper = new ApplicationInstallHelper(rsapiClient, applicationInstallManager,
					libraryApplicationManager, ConfigurationHelper.SERVER_BINDING_TYPE,
					ConfigurationHelper.REST_SERVER_ADDRESS, ConfigurationHelper.ADMIN_USERNAME,
					ConfigurationHelper.DEFAULT_PASSWORD);

				var keplerTestRapFilePath = fileLocation + @"\" + Constants.Kepler.KeplerTestRap.KEPLER_TEST_APP_NAME + ".rap";
				var fileStream = File.OpenRead(keplerTestRapFilePath);

				var keplerTestRapArtifactId = applicationInstallHelper
					.InstallApplicationAsync(Constants.Kepler.KeplerTestRap.KEPLER_TEST_APP_NAME, fileStream, -1, true).Result;

				return keplerTestRapArtifactId;
			}
			catch (Exception ex)
			{
				throw new TestHelpersException($"{nameof(InstallKeplerTestRap)} - Application installation failed for RAP {Constants.Kepler.KeplerTestRap.KEPLER_TEST_APP_NAME} - Exception: {ex.Message}");
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
			var relativityServicesUri = new Uri($"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.RSAPI_SERVER_ADDRESS}/Relativity.Services");
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
