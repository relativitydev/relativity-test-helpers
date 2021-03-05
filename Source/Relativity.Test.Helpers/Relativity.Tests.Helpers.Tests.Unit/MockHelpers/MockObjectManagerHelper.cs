using Castle.Components.DictionaryAdapter;
using Moq;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;

namespace Relativity.Tests.Helpers.Tests.Unit.MockHelpers
{
	public static class MockObjectManagerHelper
	{
		/// <summary>
		/// Creates a Mock IUserInfoManager for the OAuth helper
		/// </summary>
		/// <returns></returns>
		public static Mock<IObjectManager> GetMockObjectManagerForOAuth()
		{
			Mock<IObjectManager> mockObjectManager = new Mock<IObjectManager>();

			QueryResult queryResult = new QueryResult()
			{
				Objects = new EditableList<RelativityObject>()
				{
					new RelativityObject()
					{
						ArtifactID = 999
					}
				}
			};

			mockObjectManager.Setup(x => x.QueryAsync(It.IsAny<int>(), It.IsAny<QueryRequest>(), It.IsAny<int>(), It.IsAny<int>()))
				.ReturnsAsync(queryResult);

			return mockObjectManager;
		}
	}
}
