using Relativity.API;

namespace Relativity.Test.Helpers.Authentication
{
	public class UserInfo : IUserInfo
	{
		public int WorkspaceUserArtifactID { get; set; }

		public int ArtifactID { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string FullName { get; set; }

		public string EmailAddress { get; set; }

		public int AuditWorkspaceUserArtifactID { get; set; }

		public int AuditArtifactID { get; set; }
	}
}
