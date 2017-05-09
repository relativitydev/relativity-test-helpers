using System;
using kCura.Relativity.Client;
using NUnit.Framework;
using Relativity.API;
using Relativity.Test.Helpers.ArtifactHelpers;
using Relativity.Test.Helpers.SharedTestHelpers;
using IServicesMgr = Relativity.Test.Helpers.Interface.IServicesMgr;

namespace Relativity.Test.Helpers
{

	/// <summary>
	/// 
	/// Relativity Integration Test Helpers to assist you with writing good Integration Tests for your application. You can use this framework to test event handlers, agents and any workflow that combines agents and frameworks.
	/// 
	/// Before you get Started, fill out details for the following the app.config file
	/// "WorkspaceID", "RSAPIServerAddress", "RESTServerAddress",	"AdminUsername","AdminPassword", "SQLServerAddress" ,"SQLUsername","SQLPassword" "TestWorkspaceName"
	/// 
	/// </summary>

	[TestFixture, Description("Fixture description here")]
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

		[TestFixtureSetUp]
		public void Execute_TestFixtureSetup()
		{
			//Setup for testing		
			var helper = new TestHelper();
			servicesManager = helper.GetServicesManager();
			_eddsDbContext = helper.GetDBContext(-1);

			//create client
			_client = helper.GetServicesManager().GetProxy<IRSAPIClient>(ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			//Create new user 
			_userArtifactId = Relativity.Test.Helpers.UserHelpers.CreateUser.CreateNewUser(_client);

			//Create new group
			Relativity.Test.Helpers.GroupHelpers.CreateGroup.Create_Group(_client, _groupName);

			//Create workspace
			_workspaceId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, servicesManager, SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
			dbContext = helper.GetDBContext(_workspaceId);
			_client.APIOptions.WorkspaceID = _workspaceId;
			_rootFolderArtifactID = Folders.GetRootFolderArtifactID(_workspaceId, servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			//Create Documents with a given folder name
			ImportAPIHelper.ImportAPIHelper.CreateDocumentswithFolderName(_workspaceId, _numberOfDocuments, true, _foldername, dbContext);

			//Create Documents with a given folder artifact id
			ImportAPIHelper.ImportAPIHelper.CreateDocumentsWithFolderArtifactID(_workspaceId, _rootFolderArtifactID, dbContext);

			//Create Fixed Length field
			_fixedLengthArtId = Relativity.Test.Helpers.ArtifactHelpers.Fields.CreateField_FixedLengthText(_client, _workspaceId);

			//Create Long Text Field
			_longtextartid = Relativity.Test.Helpers.ArtifactHelpers.Fields.CreateField_LongText(_client, _workspaceId);

			//Create Whole number field
			_wholeNumberArtId = Relativity.Test.Helpers.ArtifactHelpers.Fields.CreateField_WholeNumber(_client, _workspaceId);

			//Create Yes/no field

			//Create Single Choice fields

			//Create Multiple Choice fields

		}

		#endregion

		#region TestfixtureTeardown

		[TestFixtureTearDown]
		public void Execute_TestFixtureTeardown()
		{
			//Delete Workspace
			WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceId, servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			//Delete User
			UserHelpers.DeleteUser.Delete_User(_client, _userArtifactId);

			//Delete Group
			GroupHelpers.DeleteGroup.Delete_Group(_client, _groupArtifactId);
		}

		#endregion

		#region region Golden Flow

		[Test, Description("Test description here")]
		public void Integration_Test_Golden_Flow_Valid()
		{
			//Example for Arrange, Act, Assert

			//Arrange
			//CreateEmptyFolders();
			//countBeforeExecute = SQLHelper.GetFolderCount(_workspaceId);

			////Act
			//_Executeresult = TestHelpers.Execute.ExecuteRelativityScriptByNameThroughRsapi("Delete Empty Case Folders", _workspaceId);

			////Assert
			//Assert.AreEqual(_Executeresult, "Empty folders deleted with no errors.");
			//countAfterExecute = SQLHelper.GetFolderCount(_workspaceId);
			//Assert.AreEqual(countAfterExecute, 0);
			//Assert.AreNotEqual(countBeforeExecute, countAfterExecute);

		}

		#endregion

	}

}
