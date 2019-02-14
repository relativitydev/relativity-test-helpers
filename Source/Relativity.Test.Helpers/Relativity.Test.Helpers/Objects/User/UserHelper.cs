using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

using DTOs = kCura.Relativity.Client.DTOs;

namespace Relativity.Test.Helpers.Objects.User
{
	public class UserHelper
	{
		private TestHelper _helper;

		public UserHelper(TestHelper helper)
		{
			_helper = helper;
		}

		public int CreateNewUser()
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
			var returnPasswordCodeId = FindChoiceArtifactId(sendNewPasswordTo, "Return");
			var passwordCodeId = FindChoiceArtifactId(password, "Auto-generate password");
			var documentSkipCodeId = FindChoiceArtifactId(documentSkip, "Enabled");
			var documentSkipPreferenceCodeId = FindChoiceArtifactId(skipDefaultPreference, "Normal");
			var defaultFileTypeCodeId = FindChoiceArtifactId(defaultSelectedFileType, "Native");
			var userTypeCodeId = FindChoiceArtifactId(userType, "Internal");
			var everyoneGroupArtifactId = FindGroupArtifactId("Everyone");
			var clientArtifactId = FindClientArtifactId("Relativity Template");

			var userDto = new DTOs.User
			{
				AdvancedSearchPublicByDefault = true,
				AuthenticationData = "",
				BetaUser = false,
				ChangePassword = true,
				ChangePasswordNextLogin = false,
				ChangeSettings = true,
				Client = new DTOs.Client(clientArtifactId),
				DataFocus = 1,
				DefaultSelectedFileType = new DTOs.Choice(defaultFileTypeCodeId),
				DocumentSkip = new DTOs.Choice(documentSkipCodeId),
				EmailAddress = emailAddress,
				EnforceViewerCompatibility = true,
				FirstName = firstName,
				Groups = new List<DTOs.Group> { new kCura.Relativity.Client.DTOs.Group(everyoneGroupArtifactId) },
				ItemListPageLength = 25,
				KeyboardShortcuts = true,
				LastName = lastName,
				MaximumPasswordAge = 0,
				NativeViewerCacheAhead = true,
				PasswordAction = new DTOs.Choice(passwordCodeId),
				RelativityAccess = true,
				SendPasswordTo = new DTOs.Choice(returnPasswordCodeId),
				SkipDefaultPreference = new DTOs.Choice(documentSkipPreferenceCodeId),
				TrustedIPs = "",
				Type = new DTOs.Choice(userTypeCodeId)
			};

			WriteResultSet<DTOs.User> createResults;

			try
			{
				using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(API.ExecutionIdentity.System))
				{
					client.APIOptions.WorkspaceID = -1;
					createResults = client.Repositories.User.Create(userDto);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(errorContext, ex);
			}

			// Check for success.
			if (createResults.Success)
			{
				userArtifactId = createResults.Results[0].Artifact.ArtifactID;
			}

			return userArtifactId;
		}

		public int CreateNewUser(String firstName, String lastName, String emailAddress, String password, List<int> groupArtifactIds, Boolean relativityAccess, String userType, String clientName)
		{
			int defaultSelectedFileType = 1;
			int userTypeCodeTypeId = 3;
			int documentSkip = 1000003;
			int skipDefaultPreference = 1000004;
			int passwordCodeTypeId = 1000005;
			int sendNewPasswordTo = 1000006;

			int returnPasswordCodeId = FindChoiceArtifactId(sendNewPasswordTo, "Return");
			int passwordCodeId = FindChoiceArtifactId(passwordCodeTypeId, "Manually set password");
			int documentSkipCodeId = FindChoiceArtifactId(documentSkip, "Enabled");
			int documentSkipPreferenceCodeId = FindChoiceArtifactId(skipDefaultPreference, "Normal");
			int defaultFileTypeCodeId = FindChoiceArtifactId(defaultSelectedFileType, "Native");
			int userTypeCodeId = FindChoiceArtifactId(userTypeCodeTypeId, userType);
			int everyoneGroupArtifactId = FindGroupArtifactId("Everyone");
			int clientArtifactId = FindClientArtifactId(clientName);

			DTOs.User userDto = new DTOs.User
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
				Client = new DTOs.Client(clientArtifactId),
				DataFocus = 1,
				DefaultSelectedFileType = new DTOs.Choice(defaultFileTypeCodeId),
				DocumentSkip = new DTOs.Choice(documentSkipCodeId),
				EnforceViewerCompatibility = true,
				Groups = new List<DTOs.Group> { new kCura.Relativity.Client.DTOs.Group(everyoneGroupArtifactId) },
				ItemListPageLength = 25,
				KeyboardShortcuts = true,
				MaximumPasswordAge = 0,
				NativeViewerCacheAhead = true,
				PasswordAction = new DTOs.Choice(passwordCodeId),
				RelativityAccess = relativityAccess,
				SendPasswordTo = new DTOs.Choice(returnPasswordCodeId),
				SkipDefaultPreference = new DTOs.Choice(documentSkipPreferenceCodeId),
				TrustedIPs = "",
				Type = new DTOs.Choice(userTypeCodeId),
				Password = password
			};

			foreach (Int32 groupArtifactId in groupArtifactIds)
			{
				userDto.Groups.Add(new kCura.Relativity.Client.DTOs.Group(groupArtifactId));
			}
			WriteResultSet<DTOs.User> results;
			using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(API.ExecutionIdentity.System))
			{
				client.APIOptions.WorkspaceID = -1;
				results = client.Repositories.User.Create(userDto);
			}
			return results.Results[0].Artifact.ArtifactID;
		}

		public int FindChoiceArtifactId(int choiceType, string value)
		{
			int artifactId = 0;

			WholeNumberCondition choiceTypeCondition = new WholeNumberCondition(ChoiceFieldNames.ChoiceTypeID, NumericConditionEnum.EqualTo, (int)choiceType);
			TextCondition choiceNameCondition = new TextCondition(ChoiceFieldNames.Name, TextConditionEnum.EqualTo, value);
			CompositeCondition choiceCompositeCondition = new CompositeCondition(choiceTypeCondition, CompositeConditionEnum.And, choiceNameCondition);

			Query<DTOs.Choice> choiceQuery = new Query<DTOs.Choice>(new List<FieldValue>
			{ new
							 FieldValue(ArtifactQueryFieldNames.ArtifactID) }, choiceCompositeCondition, new List<Sort>());
			QueryResultSet<DTOs.Choice> choiceQueryResult;
			using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(API.ExecutionIdentity.System))
			{
				client.APIOptions.WorkspaceID = -1;
				choiceQueryResult = client.Repositories.Choice.Query(choiceQuery);
			}
			if (choiceQueryResult.Success && choiceQueryResult.Results.Count == 1)
			{
				artifactId = choiceQueryResult.Results.FirstOrDefault().Artifact.ArtifactID;
			}

			return artifactId;
		}

		public int FindGroupArtifactId(string group)
		{
			int artifactId = 0;

			Query<kCura.Relativity.Client.DTOs.Group> queryGroup =
					 new Query<kCura.Relativity.Client.DTOs.Group>
					 {
						 Condition = new TextCondition(GroupFieldNames.Name, TextConditionEnum.EqualTo, @group)
					 };

			queryGroup.Fields.Add(new FieldValue(ArtifactQueryFieldNames.ArtifactID));
			QueryResultSet<kCura.Relativity.Client.DTOs.Group> resultSetGroup;
			using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(API.ExecutionIdentity.System))
			{
				client.APIOptions.WorkspaceID = -1;
				resultSetGroup = client.Repositories.Group.Query(queryGroup, 0);
			}
			if (resultSetGroup.Success && resultSetGroup.Results.Count == 1)
			{
				artifactId = resultSetGroup.Results.FirstOrDefault().Artifact.ArtifactID;
			}

			return artifactId;
		}

		public int FindClientArtifactId(string group)
		{
			int artifactId = 0;

			Query<DTOs.Client> queryClient = new Query<DTOs.Client>
			{
				Condition = new TextCondition(ClientFieldNames.Name, TextConditionEnum.EqualTo, @group),
				Fields = FieldValue.AllFields
			};
			QueryResultSet<DTOs.Client> resultSetClient;
			using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(API.ExecutionIdentity.System))
			{
				client.APIOptions.WorkspaceID = -1;
				resultSetClient = client.Repositories.Client.Query(queryClient, 0);
			}
			if (resultSetClient.Success && resultSetClient.Results.Count == 1)
			{
				artifactId = resultSetClient.Results.FirstOrDefault().Artifact.ArtifactID;
			}
			return artifactId;
		}

		public bool DeleteUser(int artifactId)
		{
			var userToDelete = new DTOs.User(artifactId);
			WriteResultSet<DTOs.User> resultSet;
			try
			{
				using (var client = _helper.GetServicesManager().CreateProxy<IRSAPIClient>(API.ExecutionIdentity.System))
				{
					client.APIOptions.WorkspaceID = -1;
					resultSet = client.Repositories.User.Delete(userToDelete);
				}
				if (!resultSet.Success || resultSet.Results.Count == 0)
				{
					throw new Exception("User was not found");
				}
			}
			catch (System.Exception ex)
			{
				Console.WriteLine("An error occurred deleting for the user: {0}", ex.Message);
				return false;
			}

			return true;
		}
	}
}