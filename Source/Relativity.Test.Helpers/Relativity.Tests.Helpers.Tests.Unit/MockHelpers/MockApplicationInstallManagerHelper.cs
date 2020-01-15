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
		public static Mock<IApplicationInstallManager> GetMockApplicationInstallManager(int workspaceApplicationId)
		{
			Mock<IApplicationInstallManager> mockApplicationInstallManager = new Mock<IApplicationInstallManager>();

			InstallApplicationResponse installApplicationResponse = new InstallApplicationResponse();
			installApplicationResponse.Results = new List<InstallApplicationResult>() { new InstallApplicationResult() };
			installApplicationResponse.ApplicationIdentifier = new ObjectIdentifier();
			installApplicationResponse.ApplicationIdentifier.ArtifactID = workspaceApplicationId;

			GetInstallStatusResponse getInstallStatusResponse = new GetInstallStatusResponse();
			getInstallStatusResponse.InstallStatus = new InstallStatus();
			getInstallStatusResponse.InstallStatus.Code = InstallStatusCode.Completed;

			mockApplicationInstallManager
				.Setup(x => x.InstallApplicationAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<InstallApplicationRequest>()))
				.Returns(Task.FromResult(installApplicationResponse));

			mockApplicationInstallManager
				.Setup(x => x.GetStatusAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(Task.FromResult(getInstallStatusResponse));

			return mockApplicationInstallManager;
		}
	}
}
