using NUnit.Framework;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using IServicesMgr = Relativity.API.IServicesMgr;

namespace Relativity.Test.Helpers.Example.NUnit
{

	/// <summary>
	/// 
	/// Relativity Integration Test Helpers to assist you with writing good Integration Tests for your application. You can use this framework to test event handlers, agents and any workflow that combines agents and frameworks.
	/// 
	/// Before you get Started, fill out details for the following the app.config file or runsettings file
	/// "WorkspaceID", "RelativityInstanceAddress", "RESTServerAddress",	"AdminUsername","AdminPassword", "SQLServerAddress" ,"SQLUsername","SQLPassword" "TestWorkspaceName"
	/// 
	/// </summary>

	[TestFixture, Description("Fixture description here")]
	public class TestTemplate
	{

		#region Variables
		private int _workspaceId;
		private string _workspaceName = $"IntTest_{Guid.NewGuid()}";
		private const ExecutionIdentity EXECUTION_IDENTITY = ExecutionIdentity.CurrentUser;
		private IServicesMgr _servicesMgr;
		private Int32 _numberOfDocuments = 5;
		private string _foldername = "Test Folder";
		private string _groupName = "Test Group";
		private int _userArtifactId;
		private int _groupArtifactId;
		private int _fixedLengthArtId;
		private int _longtextartid;
		private int _wholeNumberArtId;

		#endregion


		#region TestfixtureSetup

		[OneTimeSetUp]
		public void Execute_TestFixtureSetup()
		{
			//Setup for testing
			Dictionary<string, string> configDictionary = new Dictionary<string, string>();
			foreach (string testParameterName in TestContext.Parameters.Names)
			{
				configDictionary.Add(testParameterName, TestContext.Parameters[testParameterName]);
			}
			var helper = new TestHelper(configDictionary);
			_servicesMgr = helper.GetServicesManager();

			//Create new user 
			_userArtifactId = Relativity.Test.Helpers.UserHelpers.UserHelper.Create(helper.GetServicesManager());

			//Create new group
			Relativity.Test.Helpers.GroupHelpers.GroupHelper.CreateGroup(_servicesMgr, _groupName);


			//Create workspace
			_workspaceId = Helpers.WorkspaceHelpers.WorkspaceHelpers.CreateAsync(_servicesMgr, _workspaceName, SharedTestHelpers.ConfigurationHelper.TEST_WORKSPACE_TEMPLATE_NAME).ConfigureAwait(false).GetAwaiter().GetResult();

			var executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var nativeFilePath = "";
			var nativeName = @"\\\\FakeFilePath\Natives\SampleTextFile.txt";
			if (executableLocation != null)
			{
				nativeFilePath = Path.Combine(executableLocation, nativeName);
			}
			//Create Documents with a given folder name
			Relativity.Test.Helpers.ImportAPIHelper.ImportAPIHelper.CreateDocumentswithFolderName(_workspaceId, _numberOfDocuments, _foldername, nativeFilePath);

			//Create Documents with a given folder artifact id
			Relativity.Test.Helpers.ImportAPIHelper.ImportAPIHelper.CreateDocumentswithFolderName(_workspaceId, _numberOfDocuments, _foldername, nativeFilePath);

			//Create Fixed Length field
			_fixedLengthArtId = Relativity.Test.Helpers.ArtifactHelpers.FieldHelper.CreateFieldFixedLengthText(_servicesMgr, _workspaceId);

			//Create Long Text Field
			_longtextartid = Relativity.Test.Helpers.ArtifactHelpers.FieldHelper.CreateFieldLongText(_servicesMgr, _workspaceId);

			//Create Whole number field
			_wholeNumberArtId = Relativity.Test.Helpers.ArtifactHelpers.FieldHelper.CreateFieldWholeNumber(_servicesMgr, _workspaceId);

			Guid workspaceGuid = helper.GetGuid(-1, _workspaceId, Constants.ArtifactTypeIds.Workspace);
		}

		#endregion

		#region TestfixtureTeardown


		[OneTimeTearDown]
		public void Execute_TestFixtureTeardown()
		{
			//Delete Workspace
			WorkspaceHelpers.WorkspaceHelpers.Delete(_servicesMgr, _workspaceId);

			//Delete User
			UserHelpers.UserHelper.Delete(_servicesMgr, _userArtifactId);

			//Delete Group
			GroupHelpers.GroupHelper.DeleteGroup(_servicesMgr, _groupArtifactId);
		}


		#endregion

		#region region Golden Flow

		[Test, Description("Test description here")]
		public void Integration_Test_Golden_Flow_Valid()
		{
			//Arrange

			//Act

			//Assert

		}

		#endregion

	}

}