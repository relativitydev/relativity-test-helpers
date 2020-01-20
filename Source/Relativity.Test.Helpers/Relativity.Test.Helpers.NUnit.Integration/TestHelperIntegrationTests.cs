using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;

namespace Relativity.Test.Helpers.NUnit.Integration
{
	public class TestHelperIntegrationTests
	{
		private IHelper SuT;

		[OneTimeSetUp]
		public void SetUp()
		{
			SuT = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			SuT = null;
		}

		//[Test]
		//public void GetDBContextTest()
		//{
		//	// Arrange
		//	IDBContext context;

		//	// Act
		//	context = SuT.GetDBContext(-1);
		//	context.GetConnection(true);
		//	context.ReleaseConnection();

		//	// Assert
		//	Assert.IsTrue(context.Database.Equals(TestConstants.Database.EddsDatabaseName, StringComparison.OrdinalIgnoreCase));
		//}

		[Test]
		public void GetLoggerFactoryTest()
		{
			// Arrange
			ILogFactory logFactory;

			// Act
			logFactory = SuT.GetLoggerFactory();
			logFactory.GetLogger().LogDebug("GetLoggerFactoryTest: Test Log");

			// Assert
			Assert.IsTrue(logFactory != null);
		}
	}
}
