using NUnit.Framework;
using Relativity.API;
using System;

namespace Relativity.Test.Helpers.NUnit.Integration
{
	[TestFixture]
	public class TestHelperRunSettingsIntegrationTests
	{
		private IHelper SuT;

		[OneTimeSetUp]
		public void SetUp()
		{

			SuT = new TestHelper(TestContext.CurrentContext);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			SuT = null;
		}

		[Test]
		public void GetDBContextTest()
		{
			// Arrange
			IDBContext context;

			// Act
			context = SuT.GetDBContext(-1);
			context.GetConnection(true);
			context.ReleaseConnection();

			// Assert
			Assert.IsTrue(context.Database.Equals(TestConstants.Database.EddsDatabaseName, StringComparison.OrdinalIgnoreCase));
		}

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