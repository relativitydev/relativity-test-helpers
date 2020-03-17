using Moq;
using NUnit.Framework;
using Relativity.API;
using Relativity.OAuth2Client.Exceptions;
using Relativity.Test.Helpers.Exceptions;
using Relativity.Test.Helpers.Kepler;
using Relativity.Test.Helpers.RetryHelper;
using System;
using System.Threading.Tasks;

namespace Relativity.Tests.Helpers.Tests.Unit.RetryHelper
{
	[TestFixture]
	public class RetryLogicHelperUnitTests
	{
		private Mock<IOAuth2Helper> _mockOAuth2Helper;
		private Mock<IHelper> _mockHelper;

		private RetryLogicHelper SuT;

		private const string OAuth2Id = "OAuth2Id";
		private const string OAuth2Secret = "OAuth2Secret";
		private readonly Guid AppGuid = new Guid("20378139-2b25-4c8e-9c31-4aba4cc30162");

		[SetUp]
		public void SetUp()
		{
			_mockOAuth2Helper = new Mock<IOAuth2Helper>();
			_mockHelper = new Mock<IHelper>();

			SuT = new RetryLogicHelper();
		}

		[TearDown]
		public void TearDown()
		{

		}

		[TestCase(2, 2)]
		public void RetryFunction_Return_Valid(int numberOfRetries, int delayInSeconds)
		{
			// Arrange
			int workspaceId = 10;
			int artifactId = 10;

			_mockHelper
				.SetupSequence(x => x.GetGuid(It.IsAny<int>(), It.IsAny<int>()))
				.Throws(new TestHelpersException("Fake error, needs to retry"))
				.Returns(AppGuid);

			// Act
			Guid result = SuT.RetryFunction<Guid>(numberOfRetries, delayInSeconds, () => _mockHelper.Object.GetGuid(workspaceId, artifactId));

			// Assert
			Assert.AreEqual(AppGuid, result);
		}

		[TestCase(2, 2)]
		public async Task RetryFunctionAsync_Return_Valid(int numberOfRetries, int delayInSeconds)
		{
			// Arrange
			Services.Security.Models.OAuth2Client oAuth2Client = new Services.Security.Models.OAuth2Client()
			{
				Id = OAuth2Id
			};

			_mockOAuth2Helper
				.SetupSequence(x => x.CreateOAuth2ClientAsync(It.IsAny<string>(), It.IsAny<string>()))
				.ThrowsAsync(new OAuth2ClientException("Fake error, needs to retry"))
				.ReturnsAsync(oAuth2Client);

			// Act
			Services.Security.Models.OAuth2Client result = await SuT.RetryFunctionAsync(numberOfRetries, delayInSeconds, async () => await _mockOAuth2Helper.Object.CreateOAuth2ClientAsync(OAuth2Id, OAuth2Secret));

			// Assert
			Assert.AreEqual(OAuth2Id, oAuth2Client.Id);
		}

		[TestCase(2, 2)]
		public void RetryFunction_Void_Valid(int numberOfRetries, int delayInSeconds)
		{
			// Arrange
			Task completedTask = Task.CompletedTask;

			_mockOAuth2Helper
				.SetupSequence(x => x.DeleteOAuth2ClientAsync(It.IsAny<string>()))
				.Throws(new OAuth2ClientException("Fake error, needs to retry"))
				.Returns(completedTask);

			// Act

			// Assert
			Assert.DoesNotThrow(() => SuT.RetryFunction(numberOfRetries, delayInSeconds, () => _mockOAuth2Helper.Object.DeleteOAuth2ClientAsync(OAuth2Id).Wait()));
		}

		[TestCase(2, 2)]
		public async Task RetryFunctionAsync_Void_Valid(int numberOfRetries, int delayInSeconds)
		{
			// Arrange
			await Task.Yield();
			Task completedTask = Task.CompletedTask;

			_mockOAuth2Helper
				.SetupSequence(x => x.DeleteOAuth2ClientAsync(It.IsAny<string>()))
				.Throws(new OAuth2ClientException("Fake error, needs to retry"))
				.Returns(completedTask);

			// Act

			// Assert
			Assert.DoesNotThrowAsync(async () => await SuT.RetryFunctionAsync(numberOfRetries, delayInSeconds, async () => await _mockOAuth2Helper.Object.DeleteOAuth2ClientAsync(OAuth2Id)));
		}
	}
}
