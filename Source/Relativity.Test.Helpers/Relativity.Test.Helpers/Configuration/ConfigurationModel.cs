namespace Relativity.Test.Helpers.Configuration.Models
{
	// TODO: Add basic validation
	public class ConfigurationModel
	{
		/// <summary>
		/// The artifact ID of the workspace to run your test in.
		/// </summary>
		/// <value>1017177</value>
		public int WorkspaceID { get; set; }

		private string _workspaceName;

		/// <summary>
		/// The name of the workspace to run your test in.
		/// </summary>
		/// <value>Test Workspace</value>
		public string WorkspaceName
		{
			get { return _workspaceName; }
			set { _workspaceName = value + new System.Random().Next(1, 100).ToString(); }
		}

		/// <summary>
		/// The name of the template workspace your test workspace will be built from.
		/// </summary>
		/// <value>Relativity Starter Template</value>
		public string WorkspaceTemplateName { get; set; }

		/// <summary>
		/// The user name of the API executor of your test code
		/// </summary>
		/// <value>relativity.admin@relativity.com</value>
		public string AdminUsername { get; set; }

		/// <summary>
		/// The password of the API executor of your test code
		/// </summary>
		/// <value>Test1234!</value>
		public string AdminPassword { get; set; }

		/// <summary>
		/// The user name of the DB executor of your test code
		/// </summary>
		/// <value>eddsdbo</value>
		public string SQLUserName { get; set; }

		/// <summary>
		/// The user name of the DB executor of your test code
		/// </summary>
		/// <value>Test1234!</value>
		public string SQLPassword { get; set; }

		/// <summary>
		/// The host that the SQL Server runs on. Can be an IP Address or DNS lookup.
		/// </summary>
		/// <value>192.168.137.203 or devvm</value>
		public string SQLServerAddress { get; set; }

		/// <summary>
		/// The fully qualified path to the application you are testing
		/// </summary>
		/// <value>C:\\RAP\\MyCoolApp.rap</value>
		public string ApplicationRAPPath { get; set; }

		/// <summary>
		/// The host name of the Relativity instance. The RSAPI/REST endpoint can be generated from this value and ServerHostBinding. Can be an IP Address or DNS lookup.
		/// </summary>
		/// <value>192.168.137.203 or devvm</value>
		public string ServerHostName { get; set; }

		/// <summary>
		/// The host binding of the Relativity instance. The RSAPI/REST endpoint can be generated from this value and ServerHostName.
		/// </summary>
		/// <value>http or https</value>
		public string ServerHostBinding { get; set; }

		/// <summary>
		/// The fully qualified directory to find additional applications your test may require.
		/// </summary>
		/// <value>C:\\OtherRAPs</value>
		public string RAPFilesDirectory { get; set; }

		/// <summary>
		/// The email address of the user that the AuthenticationManager will consider the current user.
		/// </summary>
		/// <value>relativity.admin@relativity.com</value>
		/// <remarks>This will be set in the UserInfo object, and is distinct from the AdminUserName to allow code that sends email notifications to select a different current user to send notifications to, such as the user testing the code.</remarks>
		public string EmailAddress { get; set; }

		/// <summary>
		///	The first name of the user that the AuthenticationManager will consider the current user.
		/// </summary>
		/// <value>Relativity</value>
		public string FirstName { get; set; }
	}
}