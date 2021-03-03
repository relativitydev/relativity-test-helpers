using kCura.Relativity.Client;
using NUnit.Framework;
using Relativity.API;
using Relativity.Services.ServiceProxy;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Relativity.Services.Interfaces.Field.Models;
using IServicesMgr = Relativity.API.IServicesMgr;

namespace Relativity.Test.Helpers.Example.NUnit
{

	/// <summary>
	/// 
	/// Relativity Integration Test Helpers to assist you with writing good Integration Tests for your application. You can use this framework to test event handlers, agents and any workflow that combines agents and frameworks.
	/// 
	/// Before you get Started, fill out details for the following the app.config file
	/// "WorkspaceID", "RSAPIServerAddress", "RESTServerAddress",	"AdminUsername","AdminPassword", "SQLServerAddress" ,"SQLUsername","SQLPassword" "TestWorkspaceName"
	/// 
	/// </summary>

	[TestFixture, Description("Example Integration Tests")]
	public class TestTemplate
	{

		#region Global Test Variables
		private int _workspaceId;
		private readonly string _workspaceName = $"IntTest_{Guid.NewGuid()}";
		private IServicesMgr _servicesMgr;
		private int _groupId;
		private Services.ServiceProxy.ServiceFactory _serviceFactory;
		private IHelper _tesHelper;

		#endregion


		#region TestFixtureSetup

		[OneTimeSetUp]
		public void Execute_TestFixtureSetup()
		{
			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}

			_tesHelper = new TestHelper(configDictionary);

			//Setup for testing
			var helper = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			_servicesMgr = helper.GetServicesManager();

			// TestHelpers formerly used RSAPI
			// _client = helper.GetServicesManager().GetProxy<IRSAPIClient>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			//use helper method to instantiate a new service factory
			_serviceFactory = GetServiceFactory();

			//Create workspace
			_workspaceId = WorkspaceHelpers.WorkspaceHelpers.CreateAsync(_servicesMgr, _workspaceName, ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME).ConfigureAwait(false).GetAwaiter().GetResult();
			// old method
			//_workspaceId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, servicesManager, SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;

			//Create new group
			var groupName = "Test Group";
			_groupId = GroupHelpers.GroupHelper.CreateGroup(_serviceFactory, groupName);
			// old method that utilized RSAPI:
			// GroupHelpers.GroupHelper.CreateGroup(_client, groupName);

			//Create a Folder
			var folderName = "Test Folder";
			int folderId = ArtifactHelpers.FoldersHelper.CreateFolder(_servicesMgr, _workspaceId, folderName);

			//Create Long Text Field
			int longTextFieldId = ArtifactHelpers.FieldHelper.CreateFieldLongText(_serviceFactory, _workspaceId);
			// old method that utilized RSAPI:
			// int longTextFieldId = ArtifactHelpers.FieldHelper.CreateFieldLongText(_client, _workspaceId);

			//Create Whole number Field
			int wholeNumberFieldId = ArtifactHelpers.FieldHelper.CreateFieldWholeNumber(_serviceFactory, _workspaceId);
			// old method that utilized RSAPI:
			// int longTextFieldId = ArtifactHelpers.FieldHelper.CreateFieldWholeNumber(_client, _workspaceId);
		}

		#endregion

		#region TestFixtureTeardown


		[OneTimeTearDown]
		public void Execute_TestFixtureTeardown()
		{
			//Delete Workspace
			WorkspaceHelpers.WorkspaceHelpers.Delete(_servicesMgr, _workspaceId);

			//Delete Group
			GroupHelpers.GroupHelper.DeleteGroup(_serviceFactory, _groupId);

			_serviceFactory = null;

			_tesHelper = null;
		}


		#endregion

		#region Integration Tests

		[Test, Description("Example Test")]
		public void Integration_Test_Golden_Flow_Valid()
		{
			// run some tests
		}

		#endregion

		//helper method to create a service factory
		private Services.ServiceProxy.ServiceFactory GetServiceFactory()
		{
			var relativityServicesUri = new Uri($"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.RELATIVITY_INSTANCE_ADDRESS}/Relativity.Services");
			var relativityRestUri = new Uri($"{ConfigurationHelper.SERVER_BINDING_TYPE}://{ConfigurationHelper.REST_SERVER_ADDRESS.ToLower().Replace("-services", "")}/Relativity.Rest/Api");

			Services.ServiceProxy.UsernamePasswordCredentials usernamePasswordCredentials = new Services.ServiceProxy.UsernamePasswordCredentials(
				ConfigurationHelper.ADMIN_USERNAME,
				ConfigurationHelper.DEFAULT_PASSWORD);

			ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(
				relativityServicesUri,
				relativityRestUri,
				usernamePasswordCredentials);

			var serviceFactory = new Services.ServiceProxy.ServiceFactory(serviceFactorySettings);

			return serviceFactory;
		}
	}

}