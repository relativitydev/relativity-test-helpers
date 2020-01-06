using Castle.Components.DictionaryAdapter;
using kCura.Relativity.Client;
using kCura.Relativity.Client.Repositories;
using Moq;
using System;
using System.Linq;
using Artifact = kCura.Relativity.Client.DTOs.Artifact;
using User = kCura.Relativity.Client.DTOs.User;

namespace Relativity.Tests.Helpers.Tests.Unit.MockHelpers
{
	public static class MockRsapiClientHelper
	{
		public static void SetupIRsapiClientBehavior(this Mock<IRSAPIClient> mockRsapiClient, int workspaceId = -1)
		{
			mockRsapiClient.Setup(p => p.APIOptions).Returns(new APIOptions(workspaceId));
			var repoGroup = ForceRepositoryGroupMock(mockRsapiClient.Object);
			mockRsapiClient.SetupGet(p => p.Repositories).Returns(repoGroup);
		}

		internal static RepositoryGroup ForceRepositoryGroupMock(IRSAPIClient helper)
		{
			var repoGroupType = typeof(RepositoryGroup);
			var repositoryCtor = repoGroupType.GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).First();
			var repoGroup = repositoryCtor.Invoke(new[] { helper }) as RepositoryGroup;
			return repoGroup;
		}

		public static void SetupQueryBehavior(this Mock<IRSAPIClient> mockRsapiClient)
		{
			Artifact userArtifact = new User()
			{
				EmailAddress = "test@test.com"
			};
			QueryResult queryResult = new QueryResult()
			{
				Success = true,
				TotalCount = 1
			};

			kCura.Relativity.Client.Artifact artifact = new kCura.Relativity.Client.Artifact(-1, (int)kCura.Relativity.Client.ArtifactType.User, 999)
			{
				ArtifactID = 999,
				Name = "test@test.com",
				Fields = new EditableList<Field>()
				{
					new Field(Guid.Empty, "Email Address", "test@test.com")
				}
			};

			queryResult.QueryArtifacts.Add(artifact);

			mockRsapiClient.Setup(p => p.Query(It.IsAny<APIOptions>(), It.Is<kCura.Relativity.Client.Query>(q => q.ArtifactTypeName == "User"), It.IsAny<int>())).Returns(queryResult);
		}
	}
}
