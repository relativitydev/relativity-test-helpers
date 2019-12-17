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

# Runsettings vs App.config
As of update 7.1.0.X of Test Helpers, you can utilize Runsettings instead of being stuck to just app.config for your testing. To quickly describe Runsettings, it's just an XML file with data to be used only when a test is being run.  Azure and other CI/CD pipelines use it to supply parameters during the test.  Normally, this would be done via an app.config file, but cloud pipelines do not allow the use these, so the switch to Runsettings is necessary.

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
