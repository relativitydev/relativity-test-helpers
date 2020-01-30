using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using Relativity.Test.Helpers.Exceptions;
using Choice = kCura.Relativity.Client.DTOs.Choice;
using User = kCura.Relativity.Client.DTOs.User;

namespace Relativity.Test.Helpers.UserHelpers
{
	public static class CreateUser
	{

		public static int CreateNewUser(IRSAPIClient client)
		{
			try
			{
				const string errorContext = "An error occured when creating a new Relativity User.";
				var userArtifactId = 0;
				var firstName = "Temp User";
				var randomGuid = Convert.ToString(Guid.NewGuid());
				var lastName = randomGuid;
				var emailAddress = "tempuser." + randomGuid + "@test.com";

				//Code Types
				const int defaultSelectedFileType = 1;
				const int userType = 3;
				const int documentSkip = 1000003;
				const int skipDefaultPreference = 1000004;
				const int password = 1000005;
				const int sendNewPasswordTo = 1000006;

				//Get the ArtifactIDs for the required Choice, Group, and Client objects.
				var returnPasswordCodeId = FindChoiceArtifactId(client, sendNewPasswordTo, "Return");
				var passwordCodeId = FindChoiceArtifactId(client, password, "Auto-generate password");
				var documentSkipCodeId = FindChoiceArtifactId(client, documentSkip, "Enabled");
				var documentSkipPreferenceCodeId = FindChoiceArtifactId(client, skipDefaultPreference, "Normal");
				var defaultFileTypeCodeId = FindChoiceArtifactId(client, defaultSelectedFileType, "Native");
				var userTypeCodeId = FindChoiceArtifactId(client, userType, "Internal");
				var everyoneGroupArtifactId = FindGroupArtifactId(client, "Everyone");
				var clientArtifactId = FindClientArtifactId(client, "Relativity Template");

				var userDto = new User
				{
					AdvancedSearchPublicByDefault = true,
					AuthenticationData = "",
					BetaUser = false,
					ChangePassword = true,
					ChangePasswordNextLogin = false,
					ChangeSettings = true,
					Client = new Client(clientArtifactId),
					DataFocus = 1,
					DefaultSelectedFileType = new Choice(defaultFileTypeCodeId),
					DocumentSkip = new Choice(documentSkipCodeId),
					EmailAddress = emailAddress,
					EnforceViewerCompatibility = true,
					FirstName = firstName,
					Groups = new List<kCura.Relativity.Client.DTOs.Group> { new kCura.Relativity.Client.DTOs.Group(everyoneGroupArtifactId) },
					ItemListPageLength = 25,
					KeyboardShortcuts = true,
					LastName = lastName,
					MaximumPasswordAge = 0,
					NativeViewerCacheAhead = true,
					PasswordAction = new Choice(passwordCodeId),
					RelativityAccess = true,
					SendPasswordTo = new Choice(returnPasswordCodeId),
					SkipDefaultPreference = new Choice(documentSkipPreferenceCodeId),
					TrustedIPs = "",
					Type = new Choice(userTypeCodeId)
				};

				WriteResultSet<User> createResults;

				try
				{
					createResults = client.Repositories.User.Create(userDto);
				}
				catch (Exception ex)
				{
					throw new TestHelpersException(errorContext, ex);
				}

				// Check for success.
				userArtifactId = EnsureSuccess<User>(createResults);

				return userArtifactId;
			}
			catch (Exception ex)
			{
				throw new TestHelpersException("Error Creating New User", ex);
			}
		}

		public static int CreateNewUser(IRSAPIClient client, String firstName, String lastName, String emailAddress, String password, List<int> groupArtifactIds, Boolean relativityAccess, String userType, String clientName)
		{
			try
			{
				const string errorContext = "An error occured when creating a new Relativity User.";
				int userArtifactId = 0;

				client.APIOptions.WorkspaceID = -1;

				int defaultSelectedFileType = 1;
				int userTypeCodeTypeId = 3;
				int documentSkip = 1000003;
				int skipDefaultPreference = 1000004;
				int passwordCodeTypeId = 1000005;
				int sendNewPasswordTo = 1000006;

				int returnPasswordCodeId = FindChoiceArtifactId(client, sendNewPasswordTo, "Return");
				int passwordCodeId = FindChoiceArtifactId(client, passwordCodeTypeId, "Manually set password");
				int documentSkipCodeId = FindChoiceArtifactId(client, documentSkip, "Enabled");
				int documentSkipPreferenceCodeId = FindChoiceArtifactId(client, skipDefaultPreference, "Normal");
				int defaultFileTypeCodeId = FindChoiceArtifactId(client, defaultSelectedFileType, "Native");
				int userTypeCodeId = FindChoiceArtifactId(client, userTypeCodeTypeId, userType);
				int everyoneGroupArtifactId = FindGroupArtifactId(client, "Everyone");
				int clientArtifactId = FindClientArtifactId(client, clientName);

				User userDto = new User
				{
					FirstName = firstName,
					LastName = lastName,
					EmailAddress = emailAddress,
					AdvancedSearchPublicByDefault = true,
					AuthenticationData = "",
					BetaUser = false,
					ChangePassword = true,
					ChangePasswordNextLogin = false,
					ChangeSettings = true,
					Client = new Client(clientArtifactId),
					DataFocus = 1,
					DefaultSelectedFileType = new Choice(defaultFileTypeCodeId),
					DocumentSkip = new Choice(documentSkipCodeId),
					EnforceViewerCompatibility = true,
					Groups = new List<kCura.Relativity.Client.DTOs.Group> { new kCura.Relativity.Client.DTOs.Group(everyoneGroupArtifactId) },
					ItemListPageLength = 25,
					KeyboardShortcuts = true,
					MaximumPasswordAge = 0,
					NativeViewerCacheAhead = true,
					PasswordAction = new Choice(passwordCodeId),
					RelativityAccess = relativityAccess,
					SendPasswordTo = new Choice(returnPasswordCodeId),
					SkipDefaultPreference = new Choice(documentSkipPreferenceCodeId),
					TrustedIPs = "",
					Type = new Choice(userTypeCodeId),
					Password = password
				};

				foreach (Int32 groupArtifactId in groupArtifactIds)
				{
					userDto.Groups.Add(new kCura.Relativity.Client.DTOs.Group(groupArtifactId));
				}

				WriteResultSet<User> createResults;

				try
				{
					createResults = client.Repositories.User.Create(userDto);
				}
				catch (Exception ex)
				{
					throw new TestHelpersException(errorContext, ex);
				}

				// Check for success.
				userArtifactId = EnsureSuccess<User>(createResults);

				return userArtifactId;
			}
			catch (Exception ex)
			{
				throw new TestHelpersException("Error Creating New User", ex);
			}
		}


		public static int FindChoiceArtifactId(IRSAPIClient proxy, int choiceType, string value)
		{
			try
			{
				int artifactId = 0;

				WholeNumberCondition choiceTypeCondition = new WholeNumberCondition(ChoiceFieldNames.ChoiceTypeID, NumericConditionEnum.EqualTo, (int)choiceType);
				TextCondition choiceNameCondition = new TextCondition(ChoiceFieldNames.Name, TextConditionEnum.EqualTo, value);
				CompositeCondition choiceCompositeCondition = new CompositeCondition(choiceTypeCondition, CompositeConditionEnum.And, choiceNameCondition);

				Query<Choice> choiceQuery = new Query<Choice>(new List<FieldValue>
				{ new
					FieldValue(ArtifactQueryFieldNames.ArtifactID) }, choiceCompositeCondition, new List<Sort>());

				QueryResultSet<Choice> choiceQueryResult = proxy.Repositories.Choice.Query(choiceQuery);

				// Check for success.
				artifactId = EnsureSuccess<Choice>(choiceQueryResult);

				return artifactId;
			}
			catch (Exception ex)
			{
				throw new TestHelpersException("Error Finding Choice Artifact", ex);
			}
		}

		public static int FindGroupArtifactId(IRSAPIClient proxy, string group)
		{
			try
			{
				int artifactId = 0;

				Query<kCura.Relativity.Client.DTOs.Group> queryGroup =
					new Query<kCura.Relativity.Client.DTOs.Group>
					{
						Condition = new TextCondition(GroupFieldNames.Name, TextConditionEnum.EqualTo, @group)
					};
				queryGroup.Fields.Add(new FieldValue(ArtifactQueryFieldNames.ArtifactID));
				QueryResultSet<kCura.Relativity.Client.DTOs.Group> resultSetGroup = proxy.Repositories.Group.Query(queryGroup, 0);

				// Check for success.
				artifactId = EnsureSuccess<kCura.Relativity.Client.DTOs.Group>(resultSetGroup);

				return artifactId;
			}
			catch (Exception ex)
			{
				throw new TestHelpersException("Error Finding Group Artifact", ex);
			}
		}

		public static int FindClientArtifactId(IRSAPIClient proxy, string group)
		{
			try
			{
				int artifactId = 0;

				Query<Client> queryClient = new Query<Client>
				{
					Condition = new TextCondition(ClientFieldNames.Name, TextConditionEnum.EqualTo, @group),
					Fields = FieldValue.AllFields
				};
				QueryResultSet<Client> resultSetClient = proxy.Repositories.Client.Query(queryClient, 0);

				// Check for success.
				artifactId = EnsureSuccess<Client>(resultSetClient);

				return artifactId;
			}
			catch (Exception ex)
			{
				throw new TestHelpersException("Error Finding Client Artifact", ex);
			}
		}

		private static int EnsureSuccess<T>(this ResultSet<T> result) where T : kCura.Relativity.Client.DTOs.Artifact
		{
			int artifactId;

			if (result == null)
			{
				throw new ArgumentNullException(nameof(result));
			}
			else if (!result.Success)
			{
				string message = result.Message;
				if (string.IsNullOrWhiteSpace(message) ||
						(message ?? string.Empty).Contains("see individual results for more details"))
				{
					message += string.Join(",", result.Results.Select(x => x.Message).Where(x => !string.IsNullOrWhiteSpace(x)));
				}

				if (string.IsNullOrWhiteSpace(message))
				{
					message = "An unknown error occurred.";
				}
				throw new TestHelpersException(message);
			}
			else
			{
				if (result.Results.Count == 0)
				{
					throw new TestHelpersException($"Did not return any results [{nameof(result)}:{result}]");
				}
				else if (result.Results.Count > 1)
				{
					throw new TestHelpersException($"Returned more than 1 result [{nameof(result)}:{result}]");
				}
				else
				{
					artifactId = result.Results.FirstOrDefault().Artifact.ArtifactID;
					return artifactId;
				}
			}
		}
	}
}
