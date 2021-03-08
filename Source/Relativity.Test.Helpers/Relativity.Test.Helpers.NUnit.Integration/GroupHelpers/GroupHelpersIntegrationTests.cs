using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.Interfaces.Group;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace Relativity.Test.Helpers.NUnit.Integration.GroupHelpers
{
	[TestFixture]
	public class GroupHelpersIntegrationTests
	{
		private IHelper testHelper;
		private IServicesMgr _servicesMgr;
		private string groupName = "Test Helpers Integration Test Group";

		[SetUp]
		public void SetUp()
		{
			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}
			testHelper = new TestHelper(configDictionary);
			_servicesMgr = testHelper.GetServicesManager();
			CleanUpTestGroups();
		}

		[TearDown]
		public void TearDown()
		{
			testHelper = null;
			_servicesMgr = null;
		}

		[Test]
		public void CreateGroupAndDeleteGroupTest()
		{
			// Act
			int groupArtifactId = Relativity.Test.Helpers.GroupHelpers.GroupHelper.CreateGroup(_servicesMgr, groupName);
			bool deleteGroupResult = Relativity.Test.Helpers.GroupHelpers.GroupHelper.DeleteGroup(_servicesMgr, groupArtifactId);

			// Assert
			Assert.IsTrue(groupArtifactId > 0);
			Assert.IsTrue(deleteGroupResult);
		}

		public void CleanUpTestGroups()
		{
			try
			{
				using (IObjectManager objectManager = _servicesMgr.CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
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
							using (IGroupManager groupManager = _servicesMgr.CreateProxy<IGroupManager>(ExecutionIdentity.CurrentUser))
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
	}
}
