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
		private readonly Guid ApplicationGuid = new Guid("884a7b32-1bfe-4d8d-8229-6e012bfc08ef");
		private const int AdminWorkspaceId = -1;
		private const int LibraryApplicationId = 111;
		private const int WorkspaceApplicationId = 222;

		[SetUp]
		public void SetUp()
		{
			Mock<IApplicationInstallManager> mockApplicationInstallManager = MockApplicationInstallManagerHelper.GetMockApplicationInstallManager(WorkspaceApplicationId);
			Mock<ILibraryApplicationManager> mockLibraryApplicationManager = MockLibraryApplicationManagerHelper.GetMockLibraryApplicationManager(ApplicationName, ApplicationGuid, LibraryApplicationId);
			Mock<IRSAPIClient> mockRsapiClient = MockRsapiClientHelper.GetMockRsapiClient();
			Mock<HttpMessageHandler> mockHttpMessageHandler = MockHttpMessageHandlerHelper.GetMockHttpMessageHandler(BearerToken);

			Sut = new ApplicationInstallHelper(mockRsapiClient.Object, mockApplicationInstallManager.Object, mockLibraryApplicationManager.Object, Protocol, ServerAddress, Username, Password, mockHttpMessageHandler.Object);
		}

		[TearDown]
		public void TearDown()
		{
			Sut = null;
		}

		[Test]
		public async Task DoesLibraryApplicationExistAsyncTest()
		{
			// Arrange

			// Act
			bool result = await Sut.DoesLibraryApplicationExistAsync(ApplicationName);

			// Assert
			Assert.IsTrue(result);
		}
	}
}
