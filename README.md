# relativity-test-helpers-fork

**Open Source Community:** Integration testing is a software testing methodology to test the integration of two or more systems. With Relativity® applications, this typically means the integration between your Relativity application's code and one or more Relativity instances. These components are tested as a single group or organized in an iterative manner. We have created Relativity Integration Test Helpers to assist you with writing high-quality, low-effort integration tests for your Relativity application. You can use this framework to test event handlers, agents, custom pages, or any workflow that combines these Relativity extensibility points.

# Project Details

This project requires references to the Relativity® SDK. You may obtain the necessary assemblies from NuGet, by contacting Relativity support by email at support@relativity.com, by pulling them from your Relativity instance at C:\Program Files\kCura Corporation, or by installing the SDK from the [Community Portal](https://community.relativity.com/s/files).

# Branch Details

The _develop_ branch is in active development and is subject to breaking changes.
 
A build of the _master_ branch is available as a [nuget package](https://www.nuget.org/packages/RelativityDev.RelativityTestHelpers/).

# Usage

The primary objects of this project are:

* ConfigurationFactory

Integration tests require configuration to point to the correct Relativity instance with the appropriate user credentials. This project simplifies configuration through the use of an app.config file. A typical integration test contains the following app.config XML settings:

```
<configuration>
	<appSettings>
		<add key="WorkspaceID" value="0" />
		<add key="WorkspaceName" value="My App Integration Test" />
		<add key="ServerHostBinding" value="http" />
		<add key="ServerHostName" value="myhostname" />
		<add key="AdminUsername" value="my.relativity.admin@mycompanyname.com" />
		<add key="AdminPassword" value="myadminpassword" />
		<add key="SQLUsername" value="mysqlusername" />
		<add key="SQLPassword" value="mysqlpassword" />
		<add key="ApplicationRAPPath" value="thepathtomyapplicationRAPfile" />
	</appSettings>
</configuration>
```

To retrieve the app.config settings, you can use the following code snippet:

```
ConfigurationModel LocalConfig = ConfigurationFactory.ReadConfigFromAppSettings();
```

* TestHelper

This is a stand-alone implementation of the IHelper interface. A new instance of this object can be instantiated 

* ObjectHelper

* ConfigurationFactory




For a sample integration test, check the Relativity.Test.Helpers.Example project.