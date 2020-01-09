using kCura.Relativity.Client;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.Interfaces.LibraryApplication;
using Relativity.Test.Helpers.Kepler;
using Relativity.Test.Helpers.SharedTestHelpers;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.NUnit.Integration.Kepler
{
	public class ApplicationInstallHelperTests
	{
		private TestHelper _testHelper;
		private ApplicationInstallHelper Sut;

		private int _workspaceId;
		private string _rapFileName = "FakeApp.rap";
		private string _rapFilePath;


		[OneTimeSetUp]
		public void SetUp()
		{
			//executable location
			string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			_rapFilePath = Path.Combine(executableLocation, $"Files/{_rapFileName}");

			_testHelper = new TestHelper(TestContext.CurrentContext);

			IApplicationInstallManager applicationInstallManager = _testHelper.GetServicesManager().CreateProxy<IApplicationInstallManager>(ExecutionIdentity.System);
			ILibraryApplicationManager libraryApplicationManager = _testHelper.GetServicesManager().CreateProxy<ILibraryApplicationManager>(ExecutionIdentity.System);
			IRSAPIClient rsapiClient = _testHelper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System);

			Sut = new ApplicationInstallHelper(rsapiClient, applicationInstallManager, libraryApplicationManager);

			_workspaceId = WorkspaceHelpers.CreateWorkspace.Create(rsapiClient, ConfigurationHelper.TEST_WORKSPACE_NAME, ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			WorkspaceHelpers.DeleteWorkspace.Delete(_testHelper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System), _workspaceId);

			_testHelper = null;
			Sut = null;
		}

		[Test]
		public async Task InstallApplicationAsyncTest()
		{
			try
			{
				// Arrange
				string applicationName = "";
				//string rapFilePath = "";
				bool unlockApps = true;

				// Act
				int result = await Sut.InstallApplicationAsync(applicationName, _rapFilePath, _workspaceId, unlockApps);

				// Assert
				Assert.IsTrue(result > 0);
			}
			finally
			{

			}
		}

		[Test]
		public async Task DoesApplicationExistAsyncTest_Valid()
		{
			try
			{
				// Arrange
				//string rapFileName = ""; ;

				// Act
				bool result = await Sut.DoesApplicationExistAsync(_workspaceId, _rapFileName);

				// Assert
				Assert.IsTrue(result);
			}
			finally
			{

			}
		}

		[Test]
		public async Task DoesApplicationExistAsyncTest_Invalid()
		{
			try
			{
				// Arrange
				//string rapFileName = "";

				// Act
				bool result = await Sut.DoesApplicationExistAsync(_workspaceId, _rapFileName);

				// Assert
				Assert.IsFalse(result);
			}
			finally
			{

			}
		}

	}
}