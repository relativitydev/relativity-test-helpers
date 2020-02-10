using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace Relativity.Test.Helpers.NUnit.Integration.GroupHelpers
{
	public class GroupHelpersIntegrationTests
	{
		private IHelper testHelper;
		private IServicesMgr servicesMgr;
		private IRSAPIClient rsapiClient;
		private string groupName = "Test Helpers Integration Test Group";

		[SetUp]
		public void SetUp()
		{
			testHelper = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			servicesMgr = testHelper.GetServicesManager();
			rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System);
			CleanUpTestGroups();
		}

		[TearDown]
		public void TearDown()
		{
			testHelper = null;
			servicesMgr = null;
			rsapiClient = null;
		}

		[Test]
		public void CreateGroupAndDeleteGroupTest()
		{
			// Act
			int groupArtifactId = Relativity.Test.Helpers.GroupHelpers.CreateGroup.Create_Group(rsapiClient, groupName);
			bool deleteGroupResult = Relativity.Test.Helpers.GroupHelpers.DeleteGroup.Delete_Group(rsapiClient, groupArtifactId);

			// Assert
			Assert.IsTrue(groupArtifactId > 0);
			Assert.IsTrue(deleteGroupResult);
		}

		public void CleanUpTestGroups()
		{
			rsapiClient.APIOptions.WorkspaceID = -1;
			Condition groupQueryCondition = new TextCondition(GroupFieldNames.Name, TextConditionEnum.EqualTo, groupName);
			Query<kCura.Relativity.Client.DTOs.Group> groupQuery = new Query<kCura.Relativity.Client.DTOs.Group>(FieldValue.AllFields, groupQueryCondition, new List<Sort>());
			try
			{
				QueryResultSet<kCura.Relativity.Client.DTOs.Group> queryResultSet = rsapiClient.Repositories.Group.Query(groupQuery);
				if (!queryResultSet.Success)
				{
					throw new Exception("Failed to Query for Test Groups");
				}

				if (queryResultSet.Results.Count > 0)
				{
					List<int> groupArtifactIds = new List<int>();
					foreach (Result<kCura.Relativity.Client.DTOs.Group> groupResult in queryResultSet.Results)
					{
						groupArtifactIds.Add(groupResult.Artifact.ArtifactID);
					}

					WriteResultSet<kCura.Relativity.Client.DTOs.Group> deleteResultSet = rsapiClient.Repositories.Group.Delete(groupArtifactIds);
					if (!deleteResultSet.Success)
					{
						throw new Exception("Failed to Delete Test Groups");
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
