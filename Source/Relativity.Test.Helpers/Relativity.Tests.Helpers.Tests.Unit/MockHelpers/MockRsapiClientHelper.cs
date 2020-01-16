using Castle.Components.DictionaryAdapter;
using kCura.Relativity.Client;
using kCura.Relativity.Client.Repositories;
using Moq;
using System;
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

		public static Mock<IRSAPIClient> GetMockRsapiClientForInstall(bool isApplicationAlreadyInstalled)
		{
			Mock<IRSAPIClient> mockRsapiClient = new Mock<IRSAPIClient>();

			mockRsapiClient.SetupIRsapiClientBehavior();
			mockRsapiClient.SetupQueryBehaviorForInstall(isApplicationAlreadyInstalled);

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

		private static void SetupQueryBehaviorForInstall(this Mock<IRSAPIClient> mockRsapiClient, bool isApplicationAlreadyInstalled)
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
				Name = "ApplicationName"
			};

			rdoQueryResultInstalled.QueryArtifacts.Add(rdoResultArtifactInstalled);

			if (isApplicationAlreadyInstalled)
			{
				mockRsapiClient
					.Setup(p => p.Query(It.IsAny<APIOptions>(), It.Is<kCura.Relativity.Client.Query>(q => q.ArtifactTypeID == LibraryApplicationTypeId), It.IsAny<int>()))
					.Returns(rdoQueryResultInstalled);
			}
			else
			{
				mockRsapiClient
					.SetupSequence(p => p.Query(It.IsAny<APIOptions>(), It.Is<kCura.Relativity.Client.Query>(q => q.ArtifactTypeID == LibraryApplicationTypeId), It.IsAny<int>()))
					.Returns(rdoQueryResultNotInstalled)
					.Returns(rdoQueryResultInstalled);
			}
		}
	}
}
