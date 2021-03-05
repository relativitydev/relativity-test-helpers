using Moq;
using Relativity.Services.Interfaces.UserInfo;
using Relativity.Services.Interfaces.UserInfo.Models;

namespace Relativity.Tests.Helpers.Tests.Unit.MockHelpers
{
	public static class MockUserManagerHelper
	{
		/// <summary>
		/// Creates a Mock IUserInfoManager for the OAuth helper
		/// </summary>
		/// <returns></returns>
		public static Mock<IUserInfoManager> GetMockUserManagerForOAuth()
		{
			Mock<IUserInfoManager> mockUserInfoManager = new Mock<IUserInfoManager>();

			UserResponse userResponse = new UserResponse()
			{
				ArtifactID = 999,
				EmailAddress = "test@test.com"
			};

			mockUserInfoManager.Setup(x => x.ReadAsync(It.IsAny<int>()))
				.ReturnsAsync(userResponse);

			return mockUserInfoManager;
		}
	}
}
