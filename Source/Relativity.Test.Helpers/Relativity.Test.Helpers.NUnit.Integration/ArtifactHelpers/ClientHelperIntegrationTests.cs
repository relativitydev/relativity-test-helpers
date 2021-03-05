using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers;
using System;
using System.Collections.Generic;

namespace Relativity.Test.Helpers.NUnit.Integration.ArtifactHelpers
{
	[TestFixture]
	public class ClientHelperIntegrationTests
	{
		private IHelper _testHelper;
		private IServicesMgr _servicesMgr;
		private int _clientArtifactId;
		const string _clientName = "TestClientName";


		[SetUp]
		public void SetUp()
		{
			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}
			_testHelper = new TestHelper(configDictionary);
			_servicesMgr = _testHelper.GetServicesManager();
		}

		[TearDown]
		public void TearDown()
		{
			if (_clientArtifactId != 0)
			{
				ClientHelper.DeleteClient(_servicesMgr, _clientArtifactId);
			}

			_clientArtifactId = 0;
			_testHelper = null;
			_servicesMgr = null;
		}

		[Test]
		public void CreateClientTest()
		{
			_clientArtifactId = ClientHelper.CreateClient(_servicesMgr, _clientName);

			Assert.Greater(_clientArtifactId, 0);
		}

		[Test]
		public void CreateClientTest_Failure()
		{
			Assert.Throws<Exception>(() => ClientHelper.CreateClient(_servicesMgr, null));
		}
	}
}
