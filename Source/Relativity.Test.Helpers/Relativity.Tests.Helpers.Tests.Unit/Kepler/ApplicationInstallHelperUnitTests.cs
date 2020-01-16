using kCura.Relativity.Client;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Relativity.Services.Interfaces.LibraryApplication;
using Relativity.Test.Helpers.Kepler;
using Relativity.Tests.Helpers.Tests.Unit.MockHelpers;
using System;
using System.Net.Http;
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
		private readonly Guid ApplicationGuid = new Guid("884a7b32-1bfe-4d8d-8229-6e012bfc08ef");
		private const int AdminWorkspaceId = -1;
		private const int LibraryApplicationId = 111;
		private const int WorkspaceApplicationId = 222;
		private Mock<IApplicationInstallManager> _mockApplicationInstallManager;
		private Mock<ILibraryApplicationManager> _mockLibraryApplicationManager;
		private Mock<IRSAPIClient> _mockRsapiClient;
		private Mock<HttpMessageHandler> _mockHttpMessageHandler;


		[SetUp]
		public void SetUp()
		{
			_mockApplicationInstallManager = MockApplicationInstallManagerHelper.GetMockApplicationInstallManager(WorkspaceApplicationId);



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
			_mockRsapiClient = MockRsapiClientHelper.GetMockRsapiClientForInstall(isApplicationAlreadyInstalled);
			_mockLibraryApplicationManager = MockLibraryApplicationManagerHelper.GetMockLibraryApplicationManager(ApplicationName, ApplicationGuid, LibraryApplicationId, isApplicationAlreadyInstalled);
			_mockHttpMessageHandler = MockHttpMessageHandlerHelper.GetMockHttpMessageHandlerForRelativityVersion(relativityVersion);

			Sut = new ApplicationInstallHelper(_mockRsapiClient.Object, _mockApplicationInstallManager.Object, _mockLibraryApplicationManager.Object, Protocol, ServerAddress, Username, Password, _mockHttpMessageHandler.Object);

			// Act
			bool result = await Sut.DoesLibraryApplicationExistAsync(ApplicationName);

			// Assert
			Assert.AreEqual(isApplicationAlreadyInstalled, result);
		}
	}
}
