namespace Relativity.Test.Helpers.SharedTestHelpers
{
	public abstract class ConfigurationSettings
	{
		public abstract string TestDataLocation
		{
			get;
		}

		public abstract int WorkspaceId
		{
			get;
		}

		public abstract string RestServerAddress
		{
			get;
		}

		public abstract string RelativityInstanceAddress
		{
			get;
		}

		public abstract string AdminUserName
		{
			get;
		}

		public abstract string AdminPassword
		{
			get;
		}

		public abstract string SqlServerAddress
		{
			get;
		}

		public abstract string SqlUserName
		{
			get;
		}

		public abstract string SqlPassword
		{
			get;
		}

		public abstract string TestWorkspaceName
		{
			get;
		}

		public abstract string TestWorkspaceTemplateName
		{
			get;
		}

		public abstract string ServerBindingType
		{
			get;
		}

		public abstract string SqlConnectionString
		{
			get;
		}

		public abstract string RelAdminAuthToken
		{
			get;
		}

		public abstract string DistributedSqlAddress
		{
			get;
		}

		public abstract string UploadTestingServiceRap
		{
			get;
		}

		public abstract string TestingServiceRapPath
		{
			get;
		}

		public abstract string AgentServerAddress
		{
			get;
		}

		public abstract string ForceDbContext
		{
			get;
		}
	}
}
