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

			//Create workspace
			_workspaceId = WorkspaceHelpers.CreateWorkspace.CreateWorkspaceAsync(_workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME, servicesManager, SharedTestHelpers.ConfigurationHelper.ADMIN_USERNAME, SharedTestHelpers.ConfigurationHelper.DEFAULT_PASSWORD).Result;
			dbContext = helper.GetDBContext(_workspaceId);
			_client.APIOptions.WorkspaceID = _workspaceId;
			_rootFolderArtifactID = Folders.GetRootFolderArtifactID(_workspaceId, servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);

			//Create Documents with a given folder name
			ImportAPIHelper.ImportAPIHelper.CreateDocumentswithFolderName(_workspaceId, _numberOfDocuments, true, _foldername, dbContext);

			//Create Documents with a given folder artifact id
			ImportAPIHelper.ImportAPIHelper.CreateDocumentsWithFolderArtifactID(_workspaceId, _rootFolderArtifactID, dbContext);

		}

		#endregion

		#region TestfixtureTeardown

		[TestFixtureTearDown]
		public void Execute_TestFixtureTeardown()
		{
			WorkspaceHelpers.DeleteWorkspace.DeleteTestWorkspace(_workspaceId, servicesManager, ConfigurationHelper.ADMIN_USERNAME, ConfigurationHelper.DEFAULT_PASSWORD);
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
