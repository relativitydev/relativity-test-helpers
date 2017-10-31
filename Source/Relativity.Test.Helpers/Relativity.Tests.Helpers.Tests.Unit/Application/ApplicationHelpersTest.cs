using System;
using NUnit.Framework;
using Moq;
using kCura.Relativity.Client;
using kCura.Relativity.Client.Repositories;
using Relativity.Test.Helpers.Application;

namespace Relativity.Tests.Helpers.Tests.Unit.Application
{
    [TestFixture]
    public class ApplicationHelpersTest
    {
        [Test]
        public void ImportApplication_ProcessStateRunning_ContinuesToCheck()
        {
            //ARRANGE
            var clientMock = new Mock<IRSAPIClient>();
            clientMock.Setup(x => x.APIOptions).Returns(new APIOptions());
            clientMock
                .Setup(x => x.InstallApplication(It.IsAny<APIOptions>(), It.IsAny<AppInstallRequest>()))
                .Returns(new ProcessOperationResult { Success = true, ProcessID = Guid.NewGuid() });

            clientMock
                .SetupSequence(x => x.GetProcessState(It.IsAny<APIOptions>(), It.IsAny<Guid>()))
                .Returns(new ProcessInformation { State = ProcessStateValue.Running })
                .Returns(new ProcessInformation { State = ProcessStateValue.Completed });

            
            //ACT
            Test.Helpers.Application.ApplicationHelpers.ImportApplication(clientMock.Object, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>());

            //ASSERT
            clientMock.Verify(c => c.GetProcessState(It.IsAny<APIOptions>(), It.IsAny<Guid>()), Times.AtLeast(2));
        }

        [Test]
        public void ImportApplication_InstallFails_ThrowsException()
        {
            //ARRANGE
            var clientMock = new Mock<IRSAPIClient>();
            clientMock.Setup(x => x.APIOptions).Returns(new APIOptions());
            clientMock
                .Setup(x => x.InstallApplication(It.IsAny<APIOptions>(), It.IsAny<AppInstallRequest>()))
                .Returns(new ProcessOperationResult { Success = false, ProcessID = Guid.NewGuid() });

            clientMock
                .SetupSequence(x => x.GetProcessState(It.IsAny<APIOptions>(), It.IsAny<Guid>()))
                .Returns(new ProcessInformation { State = ProcessStateValue.Running })
                .Returns(new ProcessInformation { State = ProcessStateValue.Completed });


            //ACT
            Assert.Throws<ApplicationInstallException>(() => ApplicationHelpers.ImportApplication(clientMock.Object, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()));

            //ASSERT
        }

        [Test]
        public void ImportApplication_ProcessStateRunningToCompletedWithError_ThrowsException()
        {
            //ARRANGE
            var clientMock = new Mock<IRSAPIClient>();
            clientMock.Setup(x => x.APIOptions).Returns(new APIOptions());
            clientMock
                .Setup(x => x.InstallApplication(It.IsAny<APIOptions>(), It.IsAny<AppInstallRequest>()))
                .Returns(new ProcessOperationResult { Success = true, ProcessID = Guid.NewGuid() });

            clientMock
                .SetupSequence(x => x.GetProcessState(It.IsAny<APIOptions>(), It.IsAny<Guid>()))
                .Returns(new ProcessInformation { State = ProcessStateValue.Running })
                .Returns(new ProcessInformation { State = ProcessStateValue.CompletedWithError });


            //ACT
            Assert.Throws<ApplicationInstallException>(()=>Test.Helpers.Application.ApplicationHelpers.ImportApplication(clientMock.Object, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()));

            //ASSERT
            
        }

        [Test]
        public void ImportApplication_ProcessStateRunningToHandledException_ThrowsException()
        {
            //ARRANGE
            var clientMock = new Mock<IRSAPIClient>();
            clientMock.Setup(x => x.APIOptions).Returns(new APIOptions());
            clientMock
                .Setup(x => x.InstallApplication(It.IsAny<APIOptions>(), It.IsAny<AppInstallRequest>()))
                .Returns(new ProcessOperationResult { Success = true, ProcessID = Guid.NewGuid() });

            clientMock
                .SetupSequence(x => x.GetProcessState(It.IsAny<APIOptions>(), It.IsAny<Guid>()))
                .Returns(new ProcessInformation { State = ProcessStateValue.Running })
                .Returns(new ProcessInformation { State = ProcessStateValue.HandledException });


            //ACT
            Assert.Throws<ApplicationInstallException>(() => Test.Helpers.Application.ApplicationHelpers.ImportApplication(clientMock.Object, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()));

            //ASSERT
        }

        [Test]
        public void ImportApplication_ProcessStateRunningToUnhandledException_ThrowsException()
        {
            //ARRANGE
            var clientMock = new Mock<IRSAPIClient>();
            clientMock.Setup(x => x.APIOptions).Returns(new APIOptions());
            clientMock
                .Setup(x => x.InstallApplication(It.IsAny<APIOptions>(), It.IsAny<AppInstallRequest>()))
                .Returns(new ProcessOperationResult { Success = true, ProcessID = Guid.NewGuid() });

            clientMock
                .SetupSequence(x => x.GetProcessState(It.IsAny<APIOptions>(), It.IsAny<Guid>()))
                .Returns(new ProcessInformation { State = ProcessStateValue.Running })
                .Returns(new ProcessInformation { State = ProcessStateValue.UnhandledException });


            //ACT
            Assert.Throws<ApplicationInstallException>(() => ApplicationHelpers.ImportApplication(clientMock.Object, It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string>()));

            //ASSERT
        }
    }
}
