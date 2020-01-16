using Castle.Components.DictionaryAdapter;
using kCura.Relativity.Client;
using kCura.Relativity.Client.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Field = kCura.Relativity.Client.Field;

namespace Relativity.Tests.Helpers.Tests.Unit.MockHelpers
{
	public static class MockRsapiClientHelper
	{
		private static int LibraryApplicationTypeId = 34;
		private const int WorkspaceApplicationTypeId = 1000011;

		public static Mock<IRSAPIClient> GetMockRsapiClientForUser()
		{
			Mock<IRSAPIClient> mockRsapiClient = new Mock<IRSAPIClient>();

			mockRsapiClient.SetupIRsapiClientBehavior();
			mockRsapiClient.SetupQueryBehaviorForUser();

			return mockRsapiClient;
		}

		public static Mock<IRSAPIClient> GetMockRsapiClientForInstall(bool isApplicationAlreadyInstalled, string applicationName)
		{
			Mock<IRSAPIClient> mockRsapiClient = new Mock<IRSAPIClient>();

			mockRsapiClient.SetupIRsapiClientBehavior();
			mockRsapiClient.SetupQueryBehaviorForInstall(isApplicationAlreadyInstalled, applicationName);
			mockRsapiClient.SetupDeleteBehaviorForInstall(isApplicationAlreadyInstalled, applicationName);
			mockRsapiClient.SetupInstallBehavior();

			return mockRsapiClient;
		}

		private static void SetupIRsapiClientBehavior(this Mock<IRSAPIClient> mockRsapiClient, int workspaceId = -1)
		{
			mockRsapiClient.Setup(p => p.APIOptions).Returns(new APIOptions(workspaceId));
			var repoGroup = ForceRepositoryGroupMock(mockRsapiClient.Object);
			mockRsapiClient.SetupGet(p => p.Repositories).Returns(repoGroup);
		}

		private static RepositoryGroup ForceRepositoryGroupMock(IRSAPIClient helper)
		{
			var repoGroupType = typeof(RepositoryGroup);
			var repositoryCtor = repoGroupType.GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).First();
			var repoGroup = repositoryCtor.Invoke(new[] { helper }) as RepositoryGroup;
			return repoGroup;
		}

		private static void SetupQueryBehaviorForUser(this Mock<IRSAPIClient> mockRsapiClient)
		{
			QueryResult userQueryResult = new QueryResult()
			{
				Success = true,
				TotalCount = 1
			};

			kCura.Relativity.Client.Artifact userResultArtifact = new kCura.Relativity.Client.Artifact(-1, (int)kCura.Relativity.Client.ArtifactType.User, 999)
			{
				ArtifactID = 999,
				Name = "test@test.com",
				Fields = new EditableList<Field>()
				{
					new Field(Guid.Empty, "Email Address", "test@test.com")
				}
			};

			userQueryResult.QueryArtifacts.Add(userResultArtifact);

			mockRsapiClient
				.Setup(p => p.Query(It.IsAny<APIOptions>(), It.Is<kCura.Relativity.Client.Query>(q => q.ArtifactTypeName == "User"), It.IsAny<int>()))
				.Returns(userQueryResult);
		}

		private static void SetupQueryBehaviorForInstall(this Mock<IRSAPIClient> mockRsapiClient, bool isApplicationAlreadyInstalled, string applicationName)
		{
			QueryResult rdoQueryResultInstalled = new QueryResult()
			{
				Success = true,
				TotalCount = 1
			};

			QueryResult rdoQueryResultNotInstalled = new QueryResult()
			{
				Success = true,
				TotalCount = 0
			};

			kCura.Relativity.Client.Artifact rdoResultArtifactInstalled = new kCura.Relativity.Client.Artifact(-1, (int)kCura.Relativity.Client.ArtifactType.ObjectType, 999)
			{
				ArtifactID = 999,
				Name = applicationName
			};

			rdoQueryResultInstalled.QueryArtifacts.Add(rdoResultArtifactInstalled);

			if (isApplicationAlreadyInstalled)
			{
				mockRsapiClient
					.Setup(p => p.Query(It.IsAny<APIOptions>(), It.Is<kCura.Relativity.Client.Query>(q => q.ArtifactTypeID == LibraryApplicationTypeId), It.IsAny<int>()))
					.Returns(rdoQueryResultInstalled);
				mockRsapiClient
					.Setup(p => p.Query(It.IsAny<APIOptions>(), It.Is<kCura.Relativity.Client.Query>(q => q.ArtifactTypeID == WorkspaceApplicationTypeId), It.IsAny<int>()))
					.Returns(rdoQueryResultInstalled);
			}
			else
			{
				mockRsapiClient
					.SetupSequence(p => p.Query(It.IsAny<APIOptions>(), It.Is<kCura.Relativity.Client.Query>(q => q.ArtifactTypeID == LibraryApplicationTypeId), It.IsAny<int>()))
					.Returns(rdoQueryResultNotInstalled)
					.Returns(rdoQueryResultInstalled);
				mockRsapiClient
					.SetupSequence(p => p.Query(It.IsAny<APIOptions>(), It.Is<kCura.Relativity.Client.Query>(q => q.ArtifactTypeID == WorkspaceApplicationTypeId), It.IsAny<int>()))
					.Returns(rdoQueryResultNotInstalled)
					.Returns(rdoQueryResultInstalled)
					.Returns(rdoQueryResultInstalled);
			}
		}

		private static void SetupDeleteBehaviorForInstall(this Mock<IRSAPIClient> mockRsapiClient, bool isApplicationAlreadyInstalled, string applicationName)
		{
			ResultSet rdoDeleteResultInstalled = new ResultSet()
			{
				Success = true
			};

			ResultSet rdoDeleteResultNotInstalled = new ResultSet()
			{
				Success = false
			};

			kCura.Relativity.Client.Artifact rdoResultArtifactInstalled = new kCura.Relativity.Client.Artifact(-1, (int)kCura.Relativity.Client.ArtifactType.ObjectType, 999)
			{
				ArtifactID = 999,
				Name = applicationName
			};

			if (isApplicationAlreadyInstalled)
			{
				mockRsapiClient
					.Setup(p => p.Delete(It.IsAny<APIOptions>(), It.IsAny<List<ArtifactRequest>>()))
					.Returns(rdoDeleteResultInstalled);
			}
			else
			{
				mockRsapiClient
					.SetupSequence(p => p.Delete(It.IsAny<APIOptions>(), It.IsAny<List<ArtifactRequest>>()))
					.Returns(rdoDeleteResultNotInstalled)
					.Returns(rdoDeleteResultInstalled);
			}
		}

		private static void SetupInstallBehavior(this Mock<IRSAPIClient> mockRsapiClient)
		{
			ProcessOperationResult processOperationResult = new ProcessOperationResult()
			{
				Success = true,
				ProcessID = Guid.NewGuid()
			};

			ProcessInformation processInformation = new ProcessInformation()
			{
				State = ProcessStateValue.Completed
			};

			mockRsapiClient
				.Setup(x => x.InstallApplication(It.IsAny<APIOptions>(), It.IsAny<AppInstallRequest>()))
				.Returns(processOperationResult);
			mockRsapiClient
				.Setup(x => x.GetProcessState(It.IsAny<APIOptions>(), It.IsAny<Guid>()))
				.Returns(processInformation);
		}
	}
}
