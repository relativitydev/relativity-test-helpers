using NUnit.Framework;
using Relativity.API;
using Relativity.Services;
using Relativity.Services.Interfaces.UserInfo.Models;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Test.Helpers.GroupHelpers;
using Relativity.Test.Helpers.UserHelpers;
using System;
using System.Collections.Generic;
using Relativity.Test.Helpers.SharedTestHelpers;

namespace Relativity.Test.Helpers.NUnit.Integration.UserHelpers
{
    [TestFixture]
    public class UserHelpersIntegrationTests
    {
        private IHelper _testHelper;
        private IServicesMgr _servicesMgr;

        [OneTimeSetUp]
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

        [OneTimeTearDown]
        public void TearDown()
        {
            CleanUpTempUsers();

            _testHelper = null;
            _servicesMgr = null;
        }

        [Test]
        public void CreateUserAndDeleteUserTest()
        {
            // Arrange
            CleanUpTempUsers();

            // Act
            int userArtifactId = UserHelper.Create(_servicesMgr);
            bool deleteUserResult = UserHelper.Delete(_servicesMgr, userArtifactId);

            // Assert
            Assert.IsTrue(userArtifactId > 0);
            Assert.IsTrue(deleteUserResult);
        }

        [Test]
        public void CreateCustomUserAndDeleteUserTest()
        {
            // Arrange
            CleanUpTempUsers();

            string firstName = "Test";
            string lastName = "Helper";
            string emailAddress = $"TestHelpersFakeGuy{Constants.TestHelperTemporaryValues.UserHelperTemporaryValues.EmailSuffix}";
            string password = "hunter123";
            List<int> groupArtifactIds = new List<int>()
            {
                GroupHelper.GetGroupId(_servicesMgr, "Level 1").ConfigureAwait(false).GetAwaiter().GetResult()
            };
            bool relativityAccess = true;
            string userType = Constants.TestHelperTemporaryValues.UserHelperTemporaryValues.UserType;
            string clientName = Constants.TestHelperTemporaryValues.UserHelperTemporaryValues.Client;

            // Act
            int userArtifactId = UserHelper.Create(_servicesMgr, firstName, lastName, emailAddress, password, groupArtifactIds, relativityAccess, userType, clientName);
            bool deleteUserResult = UserHelper.Delete(_servicesMgr, userArtifactId);

            // Assert
            Assert.IsTrue(userArtifactId > 0);
            Assert.IsTrue(deleteUserResult);
        }

        [Test]
        public void GetUserId()
        {
            // Arrange
            // Act
            int userId = UserHelper.GetUserId(_servicesMgr, ConfigurationHelper.ADMIN_USERNAME).ConfigureAwait(false).GetAwaiter().GetResult();


      // Assert
      Assert.IsTrue(userId > 0);
        }

        [Test]
        public void GetUserInfo()
        {
            // Arrange
            int userId = UserHelper.GetUserId(_servicesMgr, ConfigurationHelper.ADMIN_USERNAME).ConfigureAwait(false).GetAwaiter().GetResult();

            // Act
            UserResponse result = UserHelper.GetUserInfo(_servicesMgr, userId).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            Assert.IsTrue(result.EmailAddress.Equals(ConfigurationHelper.ADMIN_USERNAME, StringComparison.CurrentCultureIgnoreCase));
        }

        public void CleanUpTempUsers()
        {
            try
            {
                using (IObjectManager objectManager = _servicesMgr.CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
                {
                    QueryRequest queryRequest = new QueryRequest()
                    {
                        ObjectType = new ObjectTypeRef { ArtifactTypeID = Constants.ArtifactTypeIds.User },
                        Condition = new TextCondition("EmailAddress", TextConditionEnum.EndsWith, Constants.TestHelperTemporaryValues.UserHelperTemporaryValues.EmailSuffix).ToQueryString(),
                        Fields = new List<FieldRef>()
                        {
                            new FieldRef { Name = "EmailAddress" }
                        },
                    };
                    QueryResult result = objectManager.QueryAsync(-1, queryRequest, 1, 100).ConfigureAwait(false).GetAwaiter().GetResult();

                    foreach (RelativityObject user in result.Objects)
                    {
                        UserHelper.Delete(_servicesMgr, user.ArtifactID);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Failed to clean up temporary users {nameof(CleanUpTempUsers)} - {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }

    }
}
