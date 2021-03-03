using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Collections.Generic;

namespace Relativity.Test.Helpers.NUnit.Integration
{
	[TestFixture]
	public class TestHelperRunSettingsIntegrationTests
	{
		private IHelper SuT;
		private KeplerHelper _keplerHelper;
		private bool useDbContext;

		[OneTimeSetUp]
		public void SetUp()
		{
			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}

			SuT = new TestHelper(configDictionary);
			_keplerHelper = new KeplerHelper();

			bool isKeplerCompatible = _keplerHelper.IsVersionKeplerCompatibleAsync().ConfigureAwait(false).GetAwaiter().GetResult();
			useDbContext = !isKeplerCompatible || ConfigurationHelper.FORCE_DBCONTEXT.Trim().ToLower().Equals("true");
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			SuT = null;
		}

		[Test]
		public void GetDBContextTest()
		{
			if (useDbContext)
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