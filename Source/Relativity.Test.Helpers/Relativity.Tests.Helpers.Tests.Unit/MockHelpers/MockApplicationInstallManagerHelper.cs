using Moq;
using Relativity.Services.Interfaces.LibraryApplication;
using Relativity.Services.Interfaces.LibraryApplication.Models;
using Relativity.Services.Interfaces.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Relativity.Tests.Helpers.Tests.Unit.MockHelpers
{
	public static class MockApplicationInstallManagerHelper
	{
		public static Mock<IApplicationInstallManager> GetMockApplicationInstallManager(int workspaceApplicationId, bool isApplicationAlreadyInstalled)
		{
			Mock<IApplicationInstallManager> mockApplicationInstallManager = new Mock<IApplicationInstallManager>();

			InstallApplicationResponse installApplicationResponse = new InstallApplicationResponse();
			installApplicationResponse.Results = new List<InstallApplicationResult>() { new InstallApplicationResult() };
			installApplicationResponse.ApplicationIdentifier = new ObjectIdentifier();
			installApplicationResponse.ApplicationIdentifier.ArtifactID = workspaceApplicationId;

			GetInstallStatusResponse getInstallStatusResponseInstalled = new GetInstallStatusResponse();
			getInstallStatusResponseInstalled.InstallStatus = new InstallStatus();
			getInstallStatusResponseInstalled.InstallStatus.Code = InstallStatusCode.Completed;

			GetInstallStatusResponse getInstallStatusResponseNotInstalled = new GetInstallStatusResponse();
			getInstallStatusResponseNotInstalled.InstallStatus = new InstallStatus();
			getInstallStatusResponseNotInstalled.InstallStatus.Code = InstallStatusCode.Failed;

			mockApplicationInstallManager
				.Setup(x => x.InstallApplicationAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<InstallApplicationRequest>()))
				.Returns(Task.FromResult(installApplicationResponse));


			if (isApplicationAlreadyInstalled)
			{
				mockApplicationInstallManager
					.Setup(x => x.GetStatusAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
					.ReturnsAsync(getInstallStatusResponseInstalled);
				//mockApplicationInstallManager
				//	.Setup(x => x.GetStatusAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				//	.ReturnsAsync(getInstallStatusResponseInstalled);
			}
			else
			{
				mockApplicationInstallManager
					.SetupSequence(x => x.GetStatusAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
					.ReturnsAsync(getInstallStatusResponseNotInstalled)
					.ReturnsAsync(getInstallStatusResponseInstalled);
				//mockApplicationInstallManager
				//	.Setup(x => x.GetStatusAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				//	.ReturnsAsync(getInstallStatusResponseInstalled);
			}


			return mockApplicationInstallManager;
		}
	}
}
