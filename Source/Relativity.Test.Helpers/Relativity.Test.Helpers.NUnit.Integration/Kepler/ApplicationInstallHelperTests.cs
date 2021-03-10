using NUnit.Framework;
using Relativity.API;
using Relativity.Services.Interfaces.LibraryApplication;
using Relativity.Test.Helpers.Kepler;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.NUnit.Integration.Kepler
{
	[TestFixture]
	public class ApplicationInstallHelperTests
	{
		private TestHelper _testHelper;
		private ApplicationInstallHelper Sut;

		private int _workspaceId;
		private string _applicationName = "FakeApp";
		private string _rapFileName = "FakeApp.rap";
		private string _rapFilePath;
		private FileStream _fileStream;

		[SetUp]
		public void SetUp()
		{
			// bin location
			string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			_rapFilePath = Path.Combine(executableLocation, $"Files/{_rapFileName}");
			_fileStream = File.OpenRead(_rapFilePath);

			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}
			_testHelper = new TestHelper(configDictionary);

			IApplicationInstallManager applicationInstallManager = _testHelper.GetServicesManager().CreateProxy<IApplicationInstallManager>(ExecutionIdentity.System);
			ILibraryApplicationManager libraryApplicationManager = _testHelper.GetServicesManager().CreateProxy<ILibraryApplicationManager>(ExecutionIdentity.System);

			Sut = new ApplicationInstallHelper(applicationInstallManager, libraryApplicationManager, ConfigurationHelper.SERVER_BINDING_TYPE, ConfigurationHelper.RELATIVITY_INSTANCE_ADDRESS, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			string workspaceName = "Test-" + Guid.NewGuid().ToString();
			_workspaceId = Helpers.WorkspaceHelpers.WorkspaceHelpers.CreateAsync(_testHelper.GetServicesManager(), workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME).ConfigureAwait(false).GetAwaiter().GetResult();

			// Delete just in case it already exists
			Sut.DeleteApplicationFromLibraryIfItExistsAsync(_applicationName).Wait();
		}

		[TearDown]
		public void TearDown()
		{
			Helpers.WorkspaceHelpers.WorkspaceHelpers.Delete(_testHelper.GetServicesManager(), _workspaceId);
			Sut.DeleteApplicationFromLibraryIfItExistsAsync(_applicationName).Wait();

			_fileStream.Close();
			_testHelper = null;
			Sut = null;
		}

		[Test]
		public async Task InstallApplicationAsyncTest()
		{
			try
			{
				// Arrange
				bool unlockApps = true;

				// Act
				int result = await Sut.InstallApplicationAsync(_applicationName, _fileStream, _workspaceId, unlockApps);

				// Assert
				Assert.IsTrue(result > 0);
			}
			finally
			{
				await Sut.DeleteApplicationFromLibraryIfItExistsAsync(_applicationName);
			}
		}

		[Test]
		public async Task DeleteApplicationFromLibraryIfItExistsAsyncTest()
		{
			try
			{
				// Arrange
				bool unlockApps = true;
				int workspaceApplicationInstallId = await Sut.InstallApplicationAsync(_applicationName, _fileStream, _workspaceId, unlockApps);

				// Act
				await Sut.DeleteApplicationFromLibraryIfItExistsAsync(_applicationName);
				bool result = await Sut.DoesLibraryApplicationExistAsync(_applicationName);

				// Assert
				Assert.IsFalse(result);
			}
			finally
			{
				await Sut.DeleteApplicationFromLibraryIfItExistsAsync(_applicationName);
			}
		}

		[Test]
		public async Task DoesWorkspaceApplicationExistAsync_Valid()
		{
			try
			{
				// Arrange
				bool unlockApps = true;
				int workspaceApplicationInstallId = await Sut.InstallApplicationAsync(_applicationName, _fileStream, _workspaceId, unlockApps);

				// Act
				bool result = await Sut.DoesWorkspaceApplicationExistAsync(_applicationName, _workspaceId, workspaceApplicationInstallId);

				// Assert
				Assert.IsTrue(result);
			}
			finally
			{
				await Sut.DeleteApplicationFromLibraryIfItExistsAsync(_applicationName);
			}
		}

		[Test]
		public async Task DoesWorkspaceApplicationExistAsync_Invalid()
		{
			// Arrange
			int workspaceApplicationInstallId = 0;

			// Act
			bool result = await Sut.DoesWorkspaceApplicationExistAsync(_applicationName, _workspaceId, workspaceApplicationInstallId);

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public async Task DoesLibraryApplicationExistAsync_Valid()
		{
			try
			{
				// Arrange
				bool unlockApps = true;
				int workspaceApplicationInstallId = await Sut.InstallApplicationAsync(_applicationName, _fileStream, _workspaceId, unlockApps);

				// Act
				bool result = await Sut.DoesLibraryApplicationExistAsync(_applicationName);

				// Assert
				Assert.IsTrue(result);
			}
			finally
			{
				await Sut.DeleteApplicationFromLibraryIfItExistsAsync(_applicationName);
			}
		}

		[Test]
		public async Task DoesLibraryApplicationExistAsync_Invalid()
		{
			// Arrange
			// Act
			bool result = await Sut.DoesLibraryApplicationExistAsync(_applicationName);

			// Assert
			Assert.IsFalse(result);
		}
	}
}