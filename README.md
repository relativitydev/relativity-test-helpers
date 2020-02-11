# Version X Updates

Starting in Version X, TestHelpers will use the [Relativity Kepler Framework](https://platform.relativity.com/RelativityOne/Content/Kepler_framework/Kepler_framework.htm) to replace the methods in Relativity.TestHelpers that use DBContext. This was done in order to make these methods compatible with RelativityOne, and creating Kepler Services instead of using DbContext to run SQL queries is Relativity development best practice.

If you are currently using one of the changed methods (e.g. `ArtifactHelpers.Fields.GetFieldArtifactID(yourFieldname, yourDbContext);`), the method will automatically use a Kepler service instead of DbContext, and you will not have to change your tests. Alternatively, you can still force the use of DbContext and not use Kepler by setting the new `app.config` value ForceDbContext to true like so: `<add key="ForceDbContext" value="true" />.` If you wish to remove DbContext from your tests entirely, you can call the corresponding overloaded method: `ArtifactHelpers.Fields.GetFieldArtifactID(yourFieldName, yourWorkspaceId, new Keplerhelper());`

In order to use Kepler services, an empty application `TestHelpers_Kepler_App.rap` will be automatically uploaded to the Application Library of your test environment. `TestHelpersKepler.Interfaces.dll` and `TestHelpersKepler.Services.dll` will also be automatically uploaded to the resource files and linked to the `TestHelpers_Kepler_App.rap`.

List of changed methods:

```csharp
Relativity.Test.Helpers.ArtifactHelpers.Document.GetDocumentIdentifierFieldColumnName(IDBContext workspaceDbContext, Int32 fieldArtifactTypeID)

Relativity.Test.Helpers.ArtifactHelpers.Document.GetDocumentIdentifierFieldName(IDBContext workspaceDbContext, Int32 fieldArtifactTypeID)

Relativity.Test.Helpers.ArtifactHelpers.Field.GetFieldArtifactID(String fieldname, IDBContext workspaceDbContext)

Relativity.Test.Helpers.ArtifactHelpers.Field.GetFieldCount(IDBContext workspaceDbContext, int fieldArtifactId)

Relativity.Test.Helpers.ArtifactHelpers.Folder.GetFolderName(int folderArtifactId, IDBContext workspaceDbContext)

Relativity.Test.Helpers.TestHelper.GetGuid(int workspaceID, int artifactID)
```

# relativity-integration-test-helpers
Open Source Community: Integration testing is a software testing methodology used to test individual software components or units of code to verify their interaction. These components are tested as a single group or organized in an iterative manner. That said, we have created Relativity Integration Test Helpers to assist you with writing good Integration Tests for your Relativity application. You can use this framework to test event handlers, agents or any workflow that combines agents and Eventhandlers. We will continue adding more helpers but in the mean time you should be able to create workspaces, create dbcontext, proxy and create documents with this framework.
 This framework is only compatible for Relativity 9.5 and above.
 
This is also available as a [nuget package].(https://www.nuget.org/packages/RelativityDev.RelativityTestHelpers/)
 
This project requires references to Relativity's Relativity® SDK dlls. These dlls are not part of the open source project and can be obtained by contacting support@relativity.com, getting it from your Relativity instance, or installing the SDK from the [Community Portal](https://community.relativity.com/s/files).
Under "relativity-integration-test-helpers\Source\Relativity.Test.Helpers\" you will need to create a "Packages" folder if one does not exist and you will need to add the following dlls to this folder. Once you do that you should be able to run your integration tests against an environment.

• FreeImageNET.dll  
• kCura.Relativity.Client.dll  
• Relativity.API.dll  
• Ionic.Zip.dll  
• itextsharp.dll  
• kCura.Data.dll  
• kCura.Data.RowDataGateway.dll  
• kCura.ImageValidator.dll  
• kCura.Injection.dll  
• kCura.OI.FileID.dll  
• kCura.Relativity.DataReaderClient.dll  
• kCura.Relativity.ImportAPI.dll  
• kCura.Utility.dll  
• kCura.Windows.Forms.dll  
• kCura.Windows.Process.dll  
• kCura.WinEDDS.dll  
• kCura.WinEDDS.ImportExtension.dll  
• Newtonsoft.Json.dll  
• Relativity.API.dll  
• Relativity.dll  
• Relativity.Services.DataContracts.dll  
• Relativity.Services.ServiceProxy.dll  



# Utilizing MailHelper for tests
## Gmail
With gmail, you will need to allow IMAP on your account.  It is recommended to make an alternate fake account just for testing.  With that, you will just need to use your gmail and gmail password in the GmailMailHelper constructor.
## MailTrap
[MailTrap Site]([https://mailtrap.io/inboxes](https://mailtrap.io/inboxes))
You will need at least a free account with MailTrap to utilize these helpers.  The free account grants you a username, password, and Api Key.  The API Key is used to access your inbox which MailTrapMailHelper takes in from its constructor.
## Licensing
[MailKit NuGet](https://github.com/jstedfast/MailKit)


# New Runsettings option within TestHelpers
As of update 7.1.0.X of Test Helpers, you can optionally utilize Runsettings instead of app.config for your testing.  Both can still be used, but just not within the same Test Fixture, as the code does a hard swap of the value collection after the TestHelpers constructor.

## Runsettings vs App.config
To quickly describe Runsettings, it's just an XML file with data to be used only when a test is being run.  Azure and other CI/CD pipelines use it to supply parameters during the test.  Normally, this would be done via an app.config file, but cloud pipelines do not allow the use of these config files, so the switch to Runsettings is necessary.

## Configuring Runsettings in Visual Studio
While in a testing project, you can simply utilize the Test dropdown menu at the top of Visual studio.  You can checkout [Microsoft's documentation on how to setup a Runsettings file](https://docs.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file?view=vs-2019) for more details.

## Setting up Test Helpers with Runsettings
The gist of the new setup is the new Runsettings file you will need in the project, and the new overloaded constructor for Test Helpers.  There are two overloads, one that takes in your Current TestContext, and one that takes in a Dictionary of values.  Both will be used to automatically map values to the static ConfigurationHelper TestHelpers creates.  You can view samples below for Runsettings files and Constructors.

**Note: For ease of use, TestHelpers expects the same parameter keys in Runsettings as it does with app.config**.

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
		<Parameter name="RSAPIServerAddress" value="172.99.99.99" />
		<Parameter name="RESTServerAddress" value="172.99.99.99" />
		<Parameter name="ServerBindingType" value="http" />
		<Parameter name="RelativityInstanceAddress" value="172.99.99.99" />
	</TestRunParameters>
</RunSettings>
```

### Sample TestHelper Constructor 1
```cs
[TestFixture]
public class TestHelperRunSettingsIntegrationTests
{
	private IHelper SuT;
	[OneTimeSetUp]
	public void SetUp()
	{
		SuT = new TestHelper(TestContext.CurrentContext);
	}
````

### Sample TestHelper Constructor 2
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

# OAuth2Helper
As of update 7.2.0.X of Test Helpers, you will have access to the OAuth2Helper object which has simple functionaliy to create and delete OAuth2 Client Credentials, as well as getting the bearer token for the created OAuth2 credential.  This should help save some time and simplify code for anything that is externally facing from a Relativity instance, such as hitting Custom Page Endpoints or Kepler Endpoints.  Admin credentials are required.

## Setup
This is easiest used in conjuction with the TestHelper object.  See the below sample code
```cs
TestHelper _testHelper = new TestHelper(TestContext.CurrentContext);

IOAuth2ClientManager oAuth2ClientManager = _testHelper.GetServicesManager().CreateProxy<IOAuth2ClientManager>(ExecutionIdentity.System);
IRSAPIClient rsapiClient = _testHelper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System);

OAuth2Helper oAuth2Helper = new OAuth2Helper(oAuth2ClientManager, rsapiClient);
````

## Usage
### Create
```cs
string oAuthName = "TestOAuth2Name";
Services.Security.Models.OAuth2Client oAuth2Client = await oAuth2Helper.CreateOAuth2ClientAsync(ConfigurationHelper.ADMIN_USERNAME, oAuthName);
````
### Delete
```cs
Services.Security.Models.OAuth2Client oAuth2Client = await oAuth2Helper.CreateOAuth2ClientAsync(ConfigurationHelper.ADMIN_USERNAME, oAuthName);
// Other code in-between here...
await oAuth2Helper.DeleteOAuth2ClientAsync(oAuth2Client.Id);
````
### Bearer Token
```cs
Services.Security.Models.OAuth2Client oAuth2Client = await oAuth2Helper.CreateOAuth2ClientAsync(ConfigurationHelper.ADMIN_USERNAME, oAuthName);

string bearerToken = await oAuth2Helper.GetBearerTokenAsync(ConfigurationHelper.SERVER_BINDING_TYPE, ConfigurationHelper.RELATIVITY_INSTANCE_ADDRESS, oAuth2Client.Id, oAuth2Client.Secret);

// Other code in-between here...

HttpClient httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

// Other HttpClient setup and overhead here...

HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
````

# ApplicationInstallHelper
As of update 7.3.0.X of Test Helpers, you will have access to the ApplicationInstallHelper object which combines both old and new ways of installing applications in one.  Based on the Relativity version of the instance you are running it against, it will either install through RSAPI or utilize the IApplicationInstallManager and ILibraryApplicationManager, but it will do it entirely on it's own, so you don't have to worry about it.  The minimum Relativity version for the new API is 10.3.170.1.

The previous Test Helper install static object and function,  ApplicationHelper.ImportApplication will still exist for backward compatibility.  

## Setup
```cs
TestHelper _testHelper = new TestHelper(TestContext.CurrentContext);

IApplicationInstallManager applicationInstallManager = _testHelper.GetServicesManager().CreateProxy<IApplicationInstallManager>(ExecutionIdentity.System);
ILibraryApplicationManager libraryApplicationManager = _testHelper.GetServicesManager().CreateProxy<ILibraryApplicationManager>(ExecutionIdentity.System);
IRSAPIClient rsapiClient = _testHelper.GetServicesManager().CreateProxy<IRSAPIClient>(ExecutionIdentity.System);

ApplicationInstallHelper applicationInstallHelper = new ApplicationInstallHelper(rsapiClient, applicationInstallManager, libraryApplicationManager, ConfigurationHelper.SERVER_BINDING_TYPE, ConfigurationHelper.RELATIVITY_INSTANCE_ADDRESS, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
````

## Usage
### Install Application
```cs
string _applicationName = "FakeApp";
string _rapFileName = "FakeApp.rap";
string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
string _rapFilePath = Path.Combine(executableLocation, $"Files/{_rapFileName}");
FileStream _fileStream = File.OpenRead(_rapFilePath);
bool unlockApps = true;

int applicationInstallId = await applicationInstallHelper.InstallApplicationAsync(_applicationName, _fileStream, _workspaceId, unlockApps);
````
### Remove (only from Library)
```cs
await applicationInstallHelper.DeleteApplicationFromLibraryIfItExistsAsync(_applicationName);
````
### Check if it's already installed (Library)
```cs
bool exists = await applicationInstallHelper.DoesLibraryApplicationExistAsync(_applicationName);
````

### Check if it's already installed (Workspace, requires the installID generated by the install code)
```cs
int workspaceApplicationInstallId = await applicationInstallHelper.InstallApplicationAsync(_applicationName, _fileStream, _workspaceId, unlockApps);

bool result = await applicationInstallHelper.DoesWorkspaceApplicationExistAsync(_applicationName, _workspaceId, workspaceApplicationInstallId);
````
