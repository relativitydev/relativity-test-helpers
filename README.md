# relativity-integration-test-helpers
Open Source Community: Integration testing is a software testing methodology used to test individual software components or units of code to verify their interaction. These components are tested as a single group or organized in an iterative manner. That said, we have created Relativity Integration Test Helpers to assist you with writing good Integration Tests for your Relativity application. You can use this framework to test event handlers, agents or any workflow that combines agents and Eventhandlers. We will continue adding more helpers but in the mean time you should be able to create workspaces, create dbcontext, proxy and create documents with this framework.

This framework is only compatible for Relativity 9.5 and above.

This is also available as a [nuget package](https://www.nuget.org/packages/RelativityDev.RelativityTestHelpers/).


# Version 8.0 Updates
With the deprecation of RSAPI, TestHelpers was significantly updated to account for this.  These updates, however, did bring about many breaking changes.

**Note: Previous packages of TestHelpers will still be available, but previous major releases will not be updated with RSAPI removal**

## Breaking Changes
Context |  v7 | v8 |
|:-----|:--------|:------|
| All | IRSAPIClient | IServicesMgr |
| All | SharedTestHelpers.ConfigurationHelper.RSAPI_SERVER_ADDRESS | SharedTestHelpers.ConfigurationHelper.RELATIVITY_INSTANCE_ADDRESS |
| Application | Application.ApplicationHelpers.ImportApplication | Kepler.ApplicationInstallHelper.InstallApplicationAsync |
| Client | ArtifactHelpers.Client.Create_Client | ArtifactHelpers.ClientHelper.CreateClient |
| Client | ArtifactHelpers.Client.Delete_Client | ArtifactHelpers.ClientHelper.DeleteClient |
| Document | ArtifactHelpers.Document.GetDocumentIdentifierFieldColumnName | Removed |
| Document | ArtifactHelpers.Document.GetDocumentIdentifierFieldName | ArtifactHelpers.DocumentHelper.GetDocumentIdentifierFieldName |
| Document | ArtifactHelpers.Document.GetDocumentIdentifierFieldColumnNameWithDbContext | Removed |
| Document | ArtifactHelpers.Document.GetDocumentIdentifierFieldNameWithDbContext | Removed |
| Field | ArtifactHelpers.Fields.GetFieldArtifactID | ArtifactHelpers.FieldHelper.GetFieldArtifactID |
| Field | ArtifactHelpers.Fields.GetFieldCount | ArtifactHelpers.FieldHelper.GetFieldCount |
| Field | ArtifactHelpers.Fields.GetFieldArtifactIDWithDbContext | Removed |
| Field | ArtifactHelpers.Fields.GetFieldCountWithDbContext | Removed |
| Field | ArtifactHelpers.Fields.CreateField | Removed |
| Field | ArtifactHelpers.Fields.CreateField_Date | ArtifactHelpers.FieldHelper.CreateFieldDate |
| Field | ArtifactHelpers.Fields.CreateField_User | ArtifactHelpers.FieldHelper.CreateFieldUser |
| Field | ArtifactHelpers.Fields.CreateField_FixedLengthText | ArtifactHelpers.FieldHelper.CreateFieldFixedLengthText |
| Field | ArtifactHelpers.Fields.CreateField_LongText | ArtifactHelpers.FieldHelper.CreateFieldLongText |
| Field | ArtifactHelpers.Fields.CreateField_WholeNumber | ArtifactHelpers.FieldHelper.CreateFieldWholeNumber |
| Field | ArtifactHelpers.Fields.CreateField_YesNO | ArtifactHelpers.FieldHelper.CreateFieldYesNo |
| Field | ArtifactHelpers.Fields.CreateField_SingleChoice | ArtifactHelpers.FieldHelper.CreateFieldSingleChoice |
| Field | ArtifactHelpers.Fields.CreateField_MultipleChoice | ArtifactHelpers.FieldHelper.CreateFieldMultipleChoice |
| Folder | ArtifactHelpers.Folder.GetFolderName | ArtifactHelpers.FoldersHelper.GetFolderName |
| Folder | ArtifactHelpers.Folder.GetRootFolderArtifactID | ArtifactHelpers.FoldersHelper.GetRootFolderArtifactID |
| Folder | ArtifactHelpers.Folder.CreateFolder | ArtifactHelpers.FoldersHelper.CreateFolder |
| Group | GroupHelpers.CreateGroup.Create_Group | GroupHelpers.GroupHelper.CreateGroup |
| Group | GroupHelpers.DeleteGroup.Delete_Group | GroupHelpers.GroupHelper.DeleteGroup |
| KeplerHelper | Any Usage | Removed |
| User | UserHelpers.CreateUser.CreateNewUser | UserHelpers.UserHelper.Create |
| User | UserHelpers.CreateUser.FindChoiceArtifactId | ArtifactHelpers.ChoiceHelper.GetChoiceId |
| User | UserHelpers.CreateUser.FindGroupArtifactId | GroupHelpers.GroupHelper.GetGroupId |
| User | UserHelpers.CreateUser.FindClientArtifactId | ArtifactHelpers.ClientHelper.GetClientId |
| User | UserHelpers.DeleteUser.Delete_User | UserHelpers.UserHelper.Delete |
| Workspace | WorkspaceHelpers.DeleteWorkspace.Delete | WorkspaceHelpers.WorkspaceHelpers.Delete |
| Workspace | WorkspaceHelpers.CreateWorkspace.Create | WorkspaceHelpers.WorkspaceHelpers.Create |
| Workspace | WorkspaceHelpers.DeleteWorkspace.Delete | WorkspaceHelpers.WorkspaceHelpers.Delete |

<br>

# New Runsettings option within TestHelpers
As of update 7.1.0.X of Test Helpers, you can optionally utilize Runsettings instead of app.config for your testing.  Both can still be used, but just not within the same Test Fixture, as the code does a hard swap of the value collection after the TestHelpers constructor.

## Runsettings vs App.config
To quickly describe Runsettings, it's just an XML file with data to be used only when a test is being run.  Azure and other CI/CD pipelines use it to supply parameters during the test.  Normally, this would be done via an app.config file, but cloud pipelines do not allow the use of these config files, so the switch to Runsettings is necessary.

## Configuring Runsettings in Visual Studio
While in a testing project, you can simply utilize the Test dropdown menu at the top of Visual studio.  You can checkout [Microsoft's documentation on how to setup a Runsettings file](https://docs.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file?view=vs-2019) for more details.

## Setting up Test Helpers with Runsettings
The gist of the new setup is the new Runsettings file you will need in the project, and the new overloaded constructor for Test Helpers.  There are two overloads, one that takes in your Current TestContext, and one that takes in a Dictionary of values.  Both will be used to automatically map values to the static ConfigurationHelper TestHelpers creates.  You can view samples below for Runsettings files and Constructors.

**Note: For ease of use, TestHelpers expects the same parameter keys in Runsettings as it does with app.config.**

### Sample Runsettings file
```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
	<!-- Parameters used by tests at run time -->
	<TestRunParameters>
		<Parameter name="WorkspaceID" value="-1" />
		<Parameter name="TestWorkspaceName" value="Test Workspace Name" />
		<Parameter name="TestWorkspaceTemplateName" value="New Case Template" />
		<Parameter name="AdminUsername" value="your.admin.account@relativity.com" />
		<Parameter name="AdminPassword" value="somepassword" />
		<Parameter name="SQLUserName" value="eddsdbo" />
		<Parameter name="SQLPassword" value="somepassword" />
		<Parameter name="SQLServerAddress" value="172.99.99.99" />
		<Parameter name="TestingServiceRapPath" value="C:\Users\user.name\Downloads" />
		<Parameter name="UploadTestingServiceRap" value="MyApp.rap" />
		<Parameter name="EmailTo" value="your.email.account@relativity.com" />
		<Parameter name="RESTServerAddress" value="172.99.99.99" />
		<Parameter name="ServerBindingType" value="http" />
		<Parameter name="RelativityInstanceAddress" value="172.99.99.99" />
	</TestRunParameters>
</RunSettings>
```

### Sample TestHelper Constructor
```cs
[TestFixture]
public class TestHelperRunSettingsIntegrationTests
{
	private IHelper SuT;
	[OneTimeSetUp]
	public void SetUp()
	{
		Dictionary<string, string> configDictionary = new Dictionary<string, string>();
		foreach (string testParameterName in TestContext.Parameters.Names)
		{
			configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
		}
		SuT = new TestHelper(configDictionary);
	}
```

# Utilizing MailHelper for tests
## Gmail
With gmail, you will need to allow IMAP on your account.  It is recommended to make an alternate fake account just for testing.  With that, you will just need to use your gmail and gmail password in the GmailMailHelper constructor.
## MailTrap
[MailTrap Site]([https://mailtrap.io/inboxes](https://mailtrap.io/inboxes))
You will need at least a free account with MailTrap to utilize these helpers.  The free account grants you a username, password, and Api Key.  The API Key is used to access your inbox which MailTrapMailHelper takes in from its constructor.
## Licensing
[MailKit NuGet](https://github.com/jstedfast/MailKit)