using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client.DTOs;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.Interfaces.Group;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.SharedTestHelpers;
using QueryResult = kCura.Relativity.Client.QueryResult;

namespace Relativity.Test.Helpers.NUnit.Integration.GroupHelpers
{
	[TestFixture]
	public class GroupHelpersIntegrationTests
	{
		private IHelper testHelper;
		private IServicesMgr servicesMgr;
		private string groupName = "Test Helpers Integration Test Group";

		private Services.ServiceProxy.ServiceFactory serviceFactory;

		[SetUp]
		public void SetUp()
		{
			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}
			testHelper = new TestHelper(configDictionary);
			servicesMgr = testHelper.GetServicesManager();
			serviceFactory = GetServiceFactory();
			CleanUpTestGroups();
		}

		[TearDown]
		public void TearDown()
		{
			testHelper = null;
			servicesMgr = null;
			serviceFactory = null;
		}

		[Test]
		public void CreateGroupAndDeleteGroupTest()
		{
			// Act
			int groupArtifactId = Relativity.Test.Helpers.GroupHelpers.GroupHelper.CreateGroup(serviceFactory, groupName);
			bool deleteGroupResult = Relativity.Test.Helpers.GroupHelpers.GroupHelper.DeleteGroup(serviceFactory, groupArtifactId);

			// Assert
			Assert.IsTrue(groupArtifactId > 0);
			Assert.IsTrue(deleteGroupResult);
		}

		public void CleanUpTestGroups()
		{
			try
			{
				using (IObjectManager objectManager = serviceFactory.CreateProxy<IObjectManager>())
				{
					QueryRequest clientQueryRequest = new QueryRequest
					{
						ObjectType = new ObjectTypeRef
						{
							ArtifactTypeID = 3
						},
						Condition = $"'Name' == '{groupName}'"
					};
					Services.Objects.DataContracts.QueryResult queryResult = objectManager.QueryAsync(-1, clientQueryRequest, 0, 10).GetAwaiter().GetResult();
					if (queryResult.TotalCount > 0)
					{
						foreach (var obj in queryResult.Objects)
						{
							using (IGroupManager groupManager = serviceFactory.CreateProxy<IGroupManager>())
							{
								groupManager.DeleteAsync(obj.ArtifactID).Wait();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error Cleaning Up Test Groups", ex);
			}
		}

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
