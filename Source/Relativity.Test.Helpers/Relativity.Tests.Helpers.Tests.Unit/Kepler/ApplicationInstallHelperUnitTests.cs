using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Relativity.Services.Interfaces.LibraryApplication;
using Relativity.Test.Helpers.Kepler;
using Relativity.Tests.Helpers.Tests.Unit.MockHelpers;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Relativity.Tests.Helpers.Tests.Unit.Kepler
{
	[TestFixture]
	public class ApplicationInstallHelperUnitTests
	{
		private IApplicationInstallHelper Sut;
		private const string BearerToken = "abc1234567890.abc1234567890abc1234567890";
		private const string Protocol = "http";
		private const string ServerAddress = "172.99.99.99";
		private const string Username = "eDiscovery";
		private const string Password = "wordpass";
		private const string ApplicationName = "FakeApp";
		private const string RelativityVersionPreKeplerApi = "10.0.318.5";
		private const string RelativityVersionPostKeplerApi = "10.3.170.1";
		private readonly Guid ApplicationGuid = new Guid("99999999-9a99-9a9a-9a99-9aaa9aa99999");
		private const int AdminWorkspaceId = -1;
		private const int LibraryApplicationId = 111;
		private const int WorkspaceApplicationId = 222;
		private Mock<IApplicationInstallManager> _mockApplicationInstallManager;
		private Mock<ILibraryApplicationManager> _mockLibraryApplicationManager;
		private Mock<HttpMessageHandler> _mockHttpMessageHandler;


		[SetUp]
		public void SetUp()
		{
			//Setup not directly done here for Sut since I use SetupSequence in the mocks and that changes per test case
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[TestCase(RelativityVersionPreKeplerApi, true)]
		[TestCase(RelativityVersionPreKeplerApi, false)]
		[TestCase(RelativityVersionPostKeplerApi, true)]
		[TestCase(RelativityVersionPostKeplerApi, false)]
		public async Task DoesLibraryApplicationExistAsyncTest(string relativityVersion, bool isApplicationAlreadyInstalled)
		{
			// Arrange
			_mockApplicationInstallManager = MockApplicationInstallManagerHelper.GetMockApplicationInstallManager(WorkspaceApplicationId, isApplicationAlreadyInstalled);
			_mockLibraryApplicationManager = MockLibraryApplicationManagerHelper.GetMockLibraryApplicationManager(ApplicationName, ApplicationGuid, LibraryApplicationId, isApplicationAlreadyInstalled);
			_mockHttpMessageHandler = MockHttpMessageHandlerHelper.GetMockHttpMessageHandlerForRelativityVersion(relativityVersion);

			Sut = new ApplicationInstallHelper(_mockApplicationInstallManager.Object, _mockLibraryApplicationManager.Object, Protocol, ServerAddress, Username, Password, _mockHttpMessageHandler.Object);

			// Act
			bool result = await Sut.DoesLibraryApplicationExistAsync(ApplicationName);

			// Assert
			Assert.AreEqual(isApplicationAlreadyInstalled, result);
		}

		[TestCase(RelativityVersionPreKeplerApi, true)]
		[TestCase(RelativityVersionPreKeplerApi, false)]
		[TestCase(RelativityVersionPostKeplerApi, true)]
		[TestCase(RelativityVersionPostKeplerApi, false)]
		public async Task DoesWorkspaceApplicationExistAsyncTest(string relativityVersion, bool isApplicationAlreadyInstalled)
		{
			// Arrange
			_mockApplicationInstallManager = MockApplicationInstallManagerHelper.GetMockApplicationInstallManager(WorkspaceApplicationId, isApplicationAlreadyInstalled);
			_mockLibraryApplicationManager = MockLibraryApplicationManagerHelper.GetMockLibraryApplicationManager(ApplicationName, ApplicationGuid, LibraryApplicationId, isApplicationAlreadyInstalled);
			_mockHttpMessageHandler = MockHttpMessageHandlerHelper.GetMockHttpMessageHandlerForRelativityVersion(relativityVersion);

			Sut = new ApplicationInstallHelper(_mockApplicationInstallManager.Object, _mockLibraryApplicationManager.Object, Protocol, ServerAddress, Username, Password, _mockHttpMessageHandler.Object);

			int workspaceId = 100999;
			int applicationId = 101010;

			// Act
			bool result = await Sut.DoesWorkspaceApplicationExistAsync(ApplicationName, workspaceId, applicationId);

			// Assert
			Assert.AreEqual(isApplicationAlreadyInstalled, result);
		}

		[TestCase(RelativityVersionPreKeplerApi, true)]
		[TestCase(RelativityVersionPreKeplerApi, false)]
		[TestCase(RelativityVersionPostKeplerApi, true)]
		[TestCase(RelativityVersionPostKeplerApi, false)]
		public async Task DeleteApplicationFromLibraryIfItExistsAsyncTest(string relativityVersion, bool isApplicationAlreadyInstalled)
		{
			// Arrange
			_mockApplicationInstallManager = MockApplicationInstallManagerHelper.GetMockApplicationInstallManager(WorkspaceApplicationId, isApplicationAlreadyInstalled);
			_mockLibraryApplicationManager = MockLibraryApplicationManagerHelper.GetMockLibraryApplicationManager(ApplicationName, ApplicationGuid, LibraryApplicationId, isApplicationAlreadyInstalled);
			_mockHttpMessageHandler = MockHttpMessageHandlerHelper.GetMockHttpMessageHandlerForRelativityVersion(relativityVersion);

			Sut = new ApplicationInstallHelper(_mockApplicationInstallManager.Object, _mockLibraryApplicationManager.Object, Protocol, ServerAddress, Username, Password, _mockHttpMessageHandler.Object);

			// Act
			await Sut.DeleteApplicationFromLibraryIfItExistsAsync(ApplicationName);
			bool result = await Sut.DoesLibraryApplicationExistAsync(ApplicationName);

			// Assert
			Assert.IsTrue(result);
		}

		[TestCase(RelativityVersionPreKeplerApi, true)]
		[TestCase(RelativityVersionPreKeplerApi, false)]
		[TestCase(RelativityVersionPostKeplerApi, true)]
		[TestCase(RelativityVersionPostKeplerApi, false)]
		public async Task InstallApplicationAsyncTest(string relativityVersion, bool isApplicationAlreadyInstalled)
		{
			// Arrange
			_mockApplicationInstallManager = MockApplicationInstallManagerHelper.GetMockApplicationInstallManager(WorkspaceApplicationId, isApplicationAlreadyInstalled);
			_mockLibraryApplicationManager = MockLibraryApplicationManagerHelper.GetMockLibraryApplicationManager(ApplicationName, ApplicationGuid, LibraryApplicationId, isApplicationAlreadyInstalled);
			_mockHttpMessageHandler = MockHttpMessageHandlerHelper.GetMockHttpMessageHandlerForRelativityVersion(relativityVersion);

			Sut = new ApplicationInstallHelper(_mockApplicationInstallManager.Object, _mockLibraryApplicationManager.Object, Protocol, ServerAddress, Username, Password, _mockHttpMessageHandler.Object);

			// bin location
			string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string rapFilePath = Path.Combine(executableLocation, "LICENSE.txt");
			FileStream fileStream = File.OpenRead(rapFilePath);
			int workspaceId = 100999;
			bool unlockApps = true;

			// Act
			int applicationInstallId = await Sut.InstallApplicationAsync(ApplicationName, fileStream, workspaceId, unlockApps);
			bool result = await Sut.DoesWorkspaceApplicationExistAsync(ApplicationName, workspaceId, applicationInstallId);

			// Assert
			Assert.IsTrue(result);
			fileStream.Close();
		}
	}
}
