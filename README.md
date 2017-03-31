# relativity-integration-test-helpers
Open Source Community: Integration testing is a software testing methodology used to test individual software components or units of code to verify their interaction. These components are tested as a single group or organized in an iterative manner.That said, we have created Relativity Integration Test Helpers to assist you with writing good Integration Tests for your Relativity application. You can use this framework to test event handlers, agents or any workflow that combines agents and Eventhandlers. We will continue adding more helpers but in the mean time you should be able to create workspaces, create dbcontext, proxy and create documents with this framework.
 This framework is only compatible for Relativity 9.5 and above.
 
This project requires references to kCura's Relativity® SDK dlls. These dlls are not part of the open source project and can be obtained by contacting support@kCura.com or getting it from your Relativity instance. 
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

