namespace Relativity.Test.Helpers.Objects
{
	public class ObjectHelper
	{
		public Application.ApplicationHelper Application { get; set; }
		public Client.ClientHelper Client { get; set; }
		public Document.DocumentHelper Document { get; set; }
		public Folder.FolderHelper Folder { get; set; }
		public Workspace.WorkspaceHelper Workspace { get; set; }
		public Group.GroupHelper Group { get; set; }
		public User.UserHelper User { get; set; }
		public Matter.MatterHelper Matter { get; set; }

		// only create via factory method
		internal ObjectHelper()	{	}
	}
}