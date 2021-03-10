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
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			SuT = null;
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