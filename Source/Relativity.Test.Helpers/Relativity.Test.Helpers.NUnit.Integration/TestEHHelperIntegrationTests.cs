using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.HelperClasses;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;

namespace Relativity.Test.Helpers.NUnit.Integration
{
	public class TestEHHelperIntegrationTests
	{
		private IEHHelper SuT;

		[OneTimeSetUp]
		public void SetUp()
		{
			SuT = new TestEHHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			SuT = null;
		}

		[Test]
		public void GetAuthenticationManagerTest()
		{
			// Arrange
			int userId = 0;

			// Act
			IAuthenticationMgr authenticationManager = SuT.GetAuthenticationManager();
			userId = authenticationManager.UserInfo.ArtifactID;
			Console.WriteLine($"GetAuthenticationManagerTest: userId = {userId}");

			// Assert
			Assert.IsTrue(userId > 0);
		}
	}
}
