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
		/// <summary>
		/// Creates some mock calls for basic use when installing an app.  Has SetupSequence for when isApplicationAlreadyInstalled is false in some cases.
		/// </summary>
		/// <param name="applicationName"></param>
		/// <param name="applicationGuid"></param>
		/// <param name="libraryApplicationId"></param>
		/// <param name="isApplicationAlreadyInstalled"></param>
		/// <returns></returns>
		public static Mock<ILibraryApplicationManager> GetMockLibraryApplicationManager(string applicationName, Guid applicationGuid, int libraryApplicationId, bool isApplicationAlreadyInstalled)
		{
			Mock<ILibraryApplicationManager> mockLibraryApplicationManager = new Mock<ILibraryApplicationManager>();

			GetInstallStatusResponse getInstallStatusResponse = new GetInstallStatusResponse();
			getInstallStatusResponse.InstallStatus = new InstallStatus();
			getInstallStatusResponse.InstallStatus.Code = InstallStatusCode.Completed;

			CreateLibraryApplicationResponse createLibraryApplicationResponse = new CreateLibraryApplicationResponse();
			createLibraryApplicationResponse.ApplicationIdentifier = new ObjectIdentifier();
			createLibraryApplicationResponse.ApplicationIdentifier.ArtifactID = libraryApplicationId;

			List<LibraryApplicationResponse> allAppsInstalled = new List<LibraryApplicationResponse>();
			allAppsInstalled.Add(new LibraryApplicationResponse()
			{
				Name = applicationName,
				Guids = new List<Guid>() { applicationGuid }
			});

			List<LibraryApplicationResponse> allAppsNotInstalled = new List<LibraryApplicationResponse>();
			allAppsNotInstalled.Add(new LibraryApplicationResponse()
			{
				Name = string.Empty,
				Guids = new List<Guid>() { Guid.Empty }
			});

			mockLibraryApplicationManager
				.Setup(x => x.GetLibraryInstallStatusAsync(It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(getInstallStatusResponse);
			mockLibraryApplicationManager
				.Setup(x => x.CreateAsync(It.IsAny<int>(), It.IsAny<IKeplerStream>()))
				.ReturnsAsync(createLibraryApplicationResponse);
			mockLibraryApplicationManager
				.Setup(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<int>()))
				.Returns(Task.CompletedTask);

			if (isApplicationAlreadyInstalled)
			{
				mockLibraryApplicationManager
					.Setup(x => x.ReadAllAsync(It.IsAny<int>()))
					.ReturnsAsync(allAppsInstalled);
			}
			else
			{
				mockLibraryApplicationManager
					.SetupSequence(x => x.ReadAllAsync(It.IsAny<int>()))
					.ReturnsAsync(allAppsNotInstalled)
					.ReturnsAsync(allAppsInstalled);
			}

			return mockLibraryApplicationManager;
		}
	}
}
