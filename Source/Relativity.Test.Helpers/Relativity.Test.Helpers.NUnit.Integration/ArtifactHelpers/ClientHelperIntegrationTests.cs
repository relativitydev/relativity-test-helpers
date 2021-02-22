using System;
using System.Collections.Generic;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace Relativity.Test.Helpers.NUnit.Integration.ArtifactHelpers
{
	[TestFixture]
	public class ClientHelperIntegrationTests
	{
		private IHelper _testHelper;
		private IServicesMgr _servicesManager;
		private Services.ServiceProxy.ServiceFactory _serviceFactory;
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
			_serviceFactory = GetServiceFactory();
		}

		[TearDown]
		public void TearDown()
		{
			if (_clientArtifactId != 0)
			{
				ClientHelper.DeleteClient(_serviceFactory, _clientArtifactId);
			}

			_clientArtifactId = 0;
			_testHelper = null;
			_servicesManager = null;
		}

		[Test]
		public void CreateClientTest()
		{
			_clientArtifactId = ClientHelper.CreateClient(_serviceFactory, _clientName);

			Assert.Greater(_clientArtifactId, 0);
		}

		[Test]
		public void CreateClientTest_Failure()
		{
			Assert.Throws<Exception>(() => ClientHelper.CreateClient(_serviceFactory, null));
		}

		//helper method
		private Services.ServiceProxy.ServiceFactory GetServiceFactory()
		{
			var relativityServicesUri = new Uri($"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.RSAPI_SERVER_ADDRESS}/Relativity.Services");
			var relativityRestUri = new Uri($"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.REST_SERVER_ADDRESS.ToLower().Replace("-services", "")}/Relativity.Rest/Api");

			Relativity.Services.ServiceProxy.UsernamePasswordCredentials usernamePasswordCredentials = new Relativity.Services.ServiceProxy.UsernamePasswordCredentials(
				username: ConfigurationHelper.ADMIN_USERNAME,
				password: ConfigurationHelper.DEFAULT_PASSWORD);

			ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(
				relativityServicesUri: relativityServicesUri,
				relativityRestUri: relativityRestUri,
				credentials: usernamePasswordCredentials);

			var serviceFactory = new Services.ServiceProxy.ServiceFactory(
				settings: serviceFactorySettings);

			return serviceFactory;
		}
	}
}
