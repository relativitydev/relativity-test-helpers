using Moq;
using Relativity.Services.Security;
using Relativity.Services.Security.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Relativity.Tests.Helpers.Tests.Unit.MockHelpers
{
	public static class MockOAuth2ClientManagerHelper
	{
		public static Mock<IOAuth2ClientManager> GetMockOAuth2ClientManager(string oAuth2Id, string oAuth2Name, string oAuth2Secret, int contextUser)
		{
			Mock<IOAuth2ClientManager> mockOAuth2ClientManager = new Mock<IOAuth2ClientManager>();
			Services.Security.Models.OAuth2Client oAuth2Client = new Services.Security.Models.OAuth2Client()
			{
				Id = oAuth2Id,
				Name = oAuth2Name,
				Secret = oAuth2Secret,
				ContextUser = contextUser
			};
			List<Services.Security.Models.OAuth2Client> oAuth2Clients = new List<Services.Security.Models.OAuth2Client>() { oAuth2Client };

			mockOAuth2ClientManager
				.Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<OAuth2Flow>(), It.IsAny<IEnumerable<Uri>>(), It.IsAny<int>()))
				.Returns(Task.FromResult(oAuth2Client));

			mockOAuth2ClientManager
				.Setup(x => x.DeleteAsync(It.IsAny<string>()))
				.Returns(Task.CompletedTask);

			mockOAuth2ClientManager
				.Setup(x => x.ReadAllAsync())
				.Returns(Task.FromResult(oAuth2Clients));

			return mockOAuth2ClientManager;
		}
	}
}
