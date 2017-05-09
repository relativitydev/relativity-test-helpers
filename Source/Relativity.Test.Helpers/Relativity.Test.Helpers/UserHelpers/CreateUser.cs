using System;
using System.Collections.Generic;
using System.Linq;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Choice = kCura.Relativity.Client.DTOs.Choice;
using User = kCura.Relativity.Client.DTOs.User;

namespace Relativity.Test.Helpers.UserHelpers
{
	public static class CreateUser
	{

		public static int CreateNewUser(IRSAPIClient client)
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
				throw new Exception(errorContext, ex);
			}

			// Check for success.
			if (createResults.Success)
			{
				userArtifactId = createResults.Results[0].Artifact.ArtifactID;
			}

			return userArtifactId;
		}

		public static int CreateNewUser(IRSAPIClient client, String firstName, String lastName, String emailAddress, String password, List<int> groupArtifactIds, Boolean relativityAccess, String userType, String clientName)
		{

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

			var results = client.Repositories.User.Create(userDto);
			return results.Results[0].Artifact.ArtifactID;
		}


		public static int FindChoiceArtifactId(IRSAPIClient proxy, int choiceType, string value)
		{
			int artifactId = 0;

			WholeNumberCondition choiceTypeCondition = new WholeNumberCondition(ChoiceFieldNames.ChoiceTypeID, NumericConditionEnum.EqualTo, (int)choiceType);
			TextCondition choiceNameCondition = new TextCondition(ChoiceFieldNames.Name, TextConditionEnum.EqualTo, value);
			CompositeCondition choiceCompositeCondition = new CompositeCondition(choiceTypeCondition, CompositeConditionEnum.And, choiceNameCondition);

			Query<Choice> choiceQuery = new Query<Choice>(new List<FieldValue>
			{ new
							 FieldValue(ArtifactQueryFieldNames.ArtifactID) }, choiceCompositeCondition, new List<Sort>());

			QueryResultSet<Choice> choiceQueryResult = proxy.Repositories.Choice.Query(choiceQuery);

			if (choiceQueryResult.Success && choiceQueryResult.Results.Count == 1)
			{
				artifactId = choiceQueryResult.Results.FirstOrDefault().Artifact.ArtifactID;
			}

			return artifactId;
		}

		public static int FindGroupArtifactId(IRSAPIClient proxy, string group)
		{
			int artifactId = 0;

			Query<kCura.Relativity.Client.DTOs.Group> queryGroup =
					 new Query<kCura.Relativity.Client.DTOs.Group>
					 {
						 Condition = new TextCondition(GroupFieldNames.Name, TextConditionEnum.EqualTo, @group)
					 };

			queryGroup.Fields.Add(new FieldValue(ArtifactQueryFieldNames.ArtifactID));

			QueryResultSet<kCura.Relativity.Client.DTOs.Group> resultSetGroup = proxy.Repositories.Group.Query(queryGroup, 0);

			if (resultSetGroup.Success && resultSetGroup.Results.Count == 1)
			{
				artifactId = resultSetGroup.Results.FirstOrDefault().Artifact.ArtifactID;
			}

			return artifactId;
		}

		public static int FindClientArtifactId(IRSAPIClient proxy, string group)
		{
			int artifactId = 0;

			Query<Client> queryClient = new Query<Client>
			{
				Condition = new TextCondition(ClientFieldNames.Name, TextConditionEnum.EqualTo, @group),
				Fields = FieldValue.AllFields
			};

			QueryResultSet<Client> resultSetClient = proxy.Repositories.Client.Query(queryClient, 0);

			if (resultSetClient.Success && resultSetClient.Results.Count == 1)
			{
				artifactId = resultSetClient.Results.FirstOrDefault().Artifact.ArtifactID;
			}
			return artifactId;
		}

	}
}
