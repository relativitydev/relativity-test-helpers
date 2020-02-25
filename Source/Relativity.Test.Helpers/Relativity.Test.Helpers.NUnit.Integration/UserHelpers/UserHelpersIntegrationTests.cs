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
using User = kCura.Relativity.Client.DTOs.User;

namespace Relativity.Test.Helpers.NUnit.Integration.UserHelpers
{
	[TestFixture]
	public class UserHelpersIntegrationTests
	{
		private IHelper testHelper;
		private IServicesMgr servicesMgr;
		private IRSAPIClient rsapiClient;

		[OneTimeSetUp]
		public void SetUp()
		{
			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}
			testHelper = new TestHelper(configDictionary);
			servicesMgr = testHelper.GetServicesManager();
			rsapiClient = servicesMgr.CreateProxy<IRSAPIClient>(ExecutionIdentity.System);
			CleanUpTempUsers();
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			testHelper = null;
			servicesMgr = null;
			rsapiClient = null;
		}

		[Test]
		public void CreateUserAndDeleteUserTest()
		{
			// Act
			int _userArtifactId = Relativity.Test.Helpers.UserHelpers.CreateUser.CreateNewUser(rsapiClient);
			bool deleteUserResult = Relativity.Test.Helpers.UserHelpers.DeleteUser.Delete_User(rsapiClient, _userArtifactId);

			// Assert
			Assert.IsTrue(_userArtifactId > 0);
			Assert.IsTrue(deleteUserResult);
		}

		public void CleanUpTempUsers()
		{
			rsapiClient.APIOptions.WorkspaceID = -1;
			Condition userQueryCondition = new TextCondition(UserFieldNames.FirstName, TextConditionEnum.EqualTo, "Temp User");
			Query<User> userQuery = new Query<User>(FieldValue.AllFields, userQueryCondition, new List<Sort>());
			try
			{
				QueryResultSet<User> queryResultSet = rsapiClient.Repositories.User.Query(userQuery);
				if (!queryResultSet.Success)
				{
					throw new Exception("Failed to Query for Temp Users");
				}

				if (queryResultSet.Results.Count > 0)
				{
					List<int> userArtifactIds = new List<int>();
					foreach (Result<User> userResult in queryResultSet.Results)
					{
						userArtifactIds.Add(userResult.Artifact.ArtifactID);
					}

					WriteResultSet<User> deleteResultSet = rsapiClient.Repositories.User.Delete(userArtifactIds);
					if (!deleteResultSet.Success)
					{
						throw new Exception("Failed to Delete Temp Users");
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error Cleaning Up Temp Users", ex);
			}
		}
	}
}
