using kCura.Relativity.Client;
using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.SharedTestHelpers;
using System;
using System.IO;
using System.Reflection;
using Relativity.Test.Helpers.ServiceFactory.Extentions;
using IServicesMgr = Relativity.API.IServicesMgr;
using TestHelpersKepler;
using TestHelpersKepler.Services;
using TestHelpersKepler.Interfaces;

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

	[TestFixture, global::NUnit.Framework.Description("Fixture description here")]
	public class TestTemplate
	{

		#region Variables
		private IRSAPIClient _client;
		private int _workspaceId;
		private Int32 _rootFolderArtifactID;
		private string _workspaceName = $"IntTest_{Guid.NewGuid()}";
		private const ExecutionIdentity EXECUTION_IDENTITY = ExecutionIdentity.CurrentUser;
		private IDBContext dbContext;
		private IServicesMgr servicesManager;
		private IDBContext _eddsDbContext;
		private Int32 _numberOfDocuments = 5;
		private string _foldername = "Test Folder";
		private string _groupName = "Test Group";
		private int _userArtifactId;
		private int _groupArtifactId;
		private int _fixedLengthArtId;
		private int _longtextartid;
		private int _yesnoartid;
		private int _wholeNumberArtId;

		#endregion


		#region TestfixtureSetup

		[OneTimeSetUp]
		public void Execute_TestFixtureSetup()
		{
			//Setup for testing
			var helper = new TestHelper(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
			servicesManager = helper.GetServicesManager();
			_eddsDbContext = helper.GetDBContext(-1);

			// implement_IHelper
			//create client
			_client = helper.GetServicesManager().GetProxy<IRSAPIClient>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			////Create new user 
			//_userArtifactId = Relativity.Test.Helpers.UserHelpers.CreateUser.CreateNewUser(_client);

			////Create new group
			//Relativity.Test.Helpers.GroupHelpers.CreateGroup.Create_Group(_client, _groupName);


			////Create workspace
			//_workspaceId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, servicesManager, SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
			//dbContext = helper.GetDBContext(_workspaceId);
			//_client.APIOptions.WorkspaceID = _workspaceId;
			//var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			//var nativeFilePath = "";
			//var nativeName = @"\\\\FakeFilePath\Natives\SampleTextFile.txt";
			//if (executableLocation != null)
			//{
			//	nativeFilePath = Path.Combine(executableLocation, nativeName);
			//}
			////Create Documents with a given folder name
			//Relativity.Test.Helpers.ImportAPIHelper.ImportAPIHelper.CreateDocumentswithFolderName(_workspaceId, _numberOfDocuments, _foldername, nativeFilePath);

			////Create Documents with a given folder artifact id
			//var folderName = Relativity.Test.Helpers.ArtifactHelpers.Folders.GetFolderName(_rootFolderArtifactID, dbContext);
			//Relativity.Test.Helpers.ImportAPIHelper.ImportAPIHelper.CreateDocumentswithFolderName(_workspaceId, _numberOfDocuments, folderName, nativeFilePath);

			////Create Fixed Length field
			//_fixedLengthArtId = Relativity.Test.Helpers.ArtifactHelpers.Fields.CreateField_FixedLengthText(_client, _workspaceId);

			////Create Long Text Field
			//_longtextartid = Relativity.Test.Helpers.ArtifactHelpers.Fields.CreateField_LongText(_client, _workspaceId);

			////Create Whole number field
			//_wholeNumberArtId = Relativity.Test.Helpers.ArtifactHelpers.Fields.CreateField_WholeNumber(_client, _workspaceId);


			////for Dt search
			//var testAppGuid = "53D74B1C-30F6-4DC2-AB64-A20150EA02D3";

			//_workspaceId = 1017834;
			//var DtSearchAppArtifactId = 1038135;

			//var guid = helper.GetGuid(_workspaceId, DtSearchAppArtifactId);

			//_workspaceId = 1017834;
			_workspaceId = -1;
			string fieldName = "Production::Begin Attachment";
			dbContext = helper.GetDBContext(_workspaceId);

			ArtifactHelpers.Fields.GetFieldArtifactID(fieldName, dbContext);

		}

		#endregion

		#region TestfixtureTeardown


		[OneTimeTearDown]
		public void Execute_TestFixtureTeardown()
		{
			//Delete Workspace
			//WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceId, servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			////Delete User
			//UserHelpers.DeleteUser.Delete_User(_client, _userArtifactId);

			////Delete Group
			//GroupHelpers.DeleteGroup.Delete_Group(_client, _groupArtifactId);
		}


		#endregion

		#region region Golden Flow

		[Test, global::NUnit.Framework.Description("Test description here")]
		public void Integration_Test_Golden_Flow_Valid()
		{
			//Arrange

			//Act

			//Assert

		}

		#endregion

	}

}