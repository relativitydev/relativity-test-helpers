using Relativity.API;
using Relativity.Services;
using Relativity.Services.Interfaces.Shared;
using Relativity.Services.Interfaces.Shared.Models;
using Relativity.Services.Interfaces.UserInfo;
using Relativity.Services.Interfaces.UserInfo.Models;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.Security;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.Exceptions;
using Relativity.Test.Helpers.GroupHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.UserHelpers
{
    public class UserHelper
    {
        public static async Task<int> GetUserId(IServicesMgr servicesMgr, string email)
        {
            try
            {
                using (IObjectManager objectManager = servicesMgr.CreateProxy<IObjectManager>(ExecutionIdentity.CurrentUser))
                {
                    QueryRequest queryRequest = new QueryRequest()
                    {
                        ObjectType = new ObjectTypeRef { ArtifactTypeID = Constants.ArtifactTypeIds.User },
                        Condition = new TextCondition("EmailAddress", TextConditionEnum.EqualTo, email).ToQueryString(),
                        Fields = new List<FieldRef>()
                        {
                            new FieldRef { Name = "Name" }
                        },
                    };
                    QueryResult result = await objectManager.QueryAsync(-1, queryRequest, 1, 10);

                    return result.Objects.First().ArtifactID;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Could not find User in {nameof(GetUserId)} for {nameof(email)} of {email} - {ex.Message}";
                Console.WriteLine(errorMessage);
                throw new TestHelpersException(errorMessage);
            }
        }

        public static async Task<UserResponse> GetUserInfo(IServicesMgr servicesMgr, int userId)
        {
            try
            {
                UserResponse result;

                using (IUserInfoManager userManager = servicesMgr.CreateProxy<IUserInfoManager>(ExecutionIdentity.CurrentUser))
                {
                    result = await userManager.ReadAsync(userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Could not find User in {nameof(GetUserInfo)} for {nameof(userId)} of {userId} - {ex.Message}";
                Console.WriteLine(errorMessage);
                throw new TestHelpersException(errorMessage);
            }
        }

        public static int Create(IServicesMgr servicesMgr)
        {
            int clientID = ClientHelper.GetClientId(servicesMgr, Constants.TestHelperTemporaryValues.UserHelperTemporaryValues.Client).ConfigureAwait(false).GetAwaiter().GetResult();
            int userTypeID = ChoiceHelper.GetChoiceId(servicesMgr, Constants.TestHelperTemporaryValues.UserHelperTemporaryValues.UserType).ConfigureAwait(false).GetAwaiter().GetResult();
            int userId;
            string randomGuid = Convert.ToString(Guid.NewGuid());


            UserRequest request = new UserRequest
            {
                AllowSettingsChange = true,
                Client = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = clientID }),
                DefaultFilterVisibility = true,
                DisableOnDate = new DateTime(2030, 12, 31),
                DocumentViewerProperties = new DocumentViewerProperties
                {
                    AllowDocumentSkipPreferenceChange = true,
                    AllowDocumentViewerChange = true,
                    AllowKeyboardShortcuts = true,
                    DefaultSelectedFileType = DocumentViewerFileType.Default,
                    DocumentViewer = DocumentViewer.Default,
                    SkipDefaultPreference = false
                },
                EmailAddress = Constants.TestHelperTemporaryValues.UserHelperTemporaryValues.EmailPrefix + randomGuid + Constants.TestHelperTemporaryValues.UserHelperTemporaryValues.EmailSuffix,
                EmailPreference = EmailPreference.Default,
                FirstName = Constants.TestHelperTemporaryValues.UserHelperTemporaryValues.FirstName,
                ItemListPageLength = 25,
                Keywords = string.Empty,
                LastName = randomGuid,
                Notes = string.Empty,
                RelativityAccess = true,
                SavedSearchDefaultsToPublic = true,
                TrustedIPs = string.Empty,
                Type = new ObjectIdentifier { ArtifactID = userTypeID }
            };

            using (IUserInfoManager userManager = servicesMgr.CreateProxy<IUserInfoManager>(ExecutionIdentity.CurrentUser))
            {
                try
                {
                    UserResponse userResponse = userManager.CreateAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
                    userId = userResponse.ArtifactID;
                    string info = string.Format("Created user with Artifact ID {0}", userId);
                    Console.WriteLine(info);
                }
                catch (Exception ex)
                {
                    string errorMessage = $"Failed to create user in {nameof(Create)} - {ex.Message}";
                    Console.WriteLine(errorMessage);
                    throw new TestHelpersException(errorMessage, ex);
                }
            }

            return userId;
        }

        public static int Create(IServicesMgr servicesMgr, string firstName, string lastName, string emailAddress, string password, List<int> groupArtifactIds, bool relativityAccess, string userType, string clientName)
        {
            int clientID = ClientHelper.GetClientId(servicesMgr, clientName).ConfigureAwait(false).GetAwaiter().GetResult();
            int userTypeID = ChoiceHelper.GetChoiceId(servicesMgr, userType).ConfigureAwait(false).GetAwaiter().GetResult();
            int userId;

            UserRequest request = new UserRequest
            {
                AllowSettingsChange = true,
                Client = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = clientID }),
                DefaultFilterVisibility = true,
                DisableOnDate = new DateTime(2030, 12, 31),
                DocumentViewerProperties = new DocumentViewerProperties
                {
                    AllowDocumentSkipPreferenceChange = true,
                    AllowDocumentViewerChange = true,
                    AllowKeyboardShortcuts = true,
                    DefaultSelectedFileType = DocumentViewerFileType.Default,
                    DocumentViewer = DocumentViewer.Default,
                    SkipDefaultPreference = false
                },
                EmailAddress = emailAddress,
                EmailPreference = EmailPreference.Default,
                FirstName = firstName,
                ItemListPageLength = 25,
                Keywords = string.Empty,
                LastName = lastName,
                Notes = string.Empty,
                RelativityAccess = relativityAccess,
                SavedSearchDefaultsToPublic = true,
                TrustedIPs = string.Empty,
                Type = new ObjectIdentifier { ArtifactID = userTypeID }
            };

            using (IUserInfoManager userManager = servicesMgr.CreateProxy<IUserInfoManager>(ExecutionIdentity.CurrentUser))
            using (IAuthProviderTypeManager authProviderTypeManager = servicesMgr.CreateProxy<IAuthProviderTypeManager>(ExecutionIdentity.CurrentUser))
            using (ILoginProfileManager loginProfileManager = servicesMgr.CreateProxy<ILoginProfileManager>(ExecutionIdentity.CurrentUser))
            {
                try
                {
                    UserResponse userResponse = userManager.CreateAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
                    userId = userResponse.ArtifactID;
                    string info = string.Format("Created user with Artifact ID {0}", userId);
                    Console.WriteLine(info);

                    string providerTypeName = Constants.TestHelperTemporaryValues.UserHelperTemporaryValues.AuthProviderType;
                    bool enabled = true;
                    {
                        authProviderTypeManager.UpdateAsync(providerTypeName, enabled).Wait();
                    }

                    loginProfileManager.SetPasswordAsync(userId, password);

                    foreach (int groupId in groupArtifactIds)
                    {
                        GroupHelper.AddUserToGroup(servicesMgr, groupId, userId);
                    }
                }
                catch (Exception ex)
                {
                    string errorMessage = $"Failed to create user in {nameof(Create)} - {ex.Message}";
                    Console.WriteLine(errorMessage);
                    throw new TestHelpersException(errorMessage, ex);
                }
            }

            return userId;
        }

        /// <summary>
        /// Delete a specific User by Id
        /// </summary>
        /// <param name="servicesMgr"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool Delete(IServicesMgr servicesMgr, int userId)
        {
            using (IUserInfoManager userManager = servicesMgr.CreateProxy<IUserInfoManager>(ExecutionIdentity.CurrentUser))
            {
                try
                {
                    userManager.DeleteAsync(userId).Wait();
                    string info = string.Format("Deleted user with Artifact ID {0}", userId);
                    Console.WriteLine(info);
                }
                catch (Exception ex)
                {
                    string errorMessage = $"Failed to delete user in {nameof(Delete)} - {ex.Message}";
                    Console.WriteLine(errorMessage);
                    return false;
                }
            }

            return true;
        }
    }
}
