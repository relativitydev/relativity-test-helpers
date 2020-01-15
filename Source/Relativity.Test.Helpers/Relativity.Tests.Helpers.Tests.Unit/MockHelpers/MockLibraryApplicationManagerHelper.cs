using Moq;
using Relativity.Kepler.Transport;
using Relativity.Services.Interfaces.LibraryApplication;
using Relativity.Services.Interfaces.LibraryApplication.Models;
using Relativity.Services.Interfaces.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Relativity.Tests.Helpers.Tests.Unit.MockHelpers
{
	public static class MockLibraryApplicationManagerHelper
	{
		public static Mock<ILibraryApplicationManager> GetMockLibraryApplicationManager(string applicationName, Guid applicationGuid, int libraryApplicationId)
		{
			Mock<ILibraryApplicationManager> mockLibraryApplicationManager = new Mock<ILibraryApplicationManager>();

			GetInstallStatusResponse getInstallStatusResponse = new GetInstallStatusResponse();
			getInstallStatusResponse.InstallStatus = new InstallStatus();
			getInstallStatusResponse.InstallStatus.Code = InstallStatusCode.Completed;

			CreateLibraryApplicationResponse createLibraryApplicationResponse = new CreateLibraryApplicationResponse();
			createLibraryApplicationResponse.ApplicationIdentifier = new ObjectIdentifier();
			createLibraryApplicationResponse.ApplicationIdentifier.ArtifactID = libraryApplicationId;

			List<LibraryApplicationResponse> allApps = new List<LibraryApplicationResponse>();
			allApps.Add(new LibraryApplicationResponse()
			{
				Name = applicationName,
				Guids = new List<Guid>() { applicationGuid }
			});

			mockLibraryApplicationManager
				.Setup(x => x.GetLibraryInstallStatusAsync(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(Task.FromResult(getInstallStatusResponse));
			mockLibraryApplicationManager
				.Setup(x => x.CreateAsync(It.IsAny<int>(), It.IsAny<IKeplerStream>()))
				.Returns(Task.FromResult(createLibraryApplicationResponse));
			mockLibraryApplicationManager
				.Setup(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(Task.CompletedTask);
			mockLibraryApplicationManager
				.Setup(x => x.ReadAllAsync(It.IsAny<int>()))
				.Returns(Task.FromResult(allApps));

			return mockLibraryApplicationManager;
		}
	}
}
