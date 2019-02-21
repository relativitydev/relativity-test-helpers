namespace Relativity.Test.Helpers.Objects
{
	public static class ObjectFactory
	{
		public static ObjectHelper ObjectHelperInstance(TestHelper helper)
		{
			var objectHelper = new ObjectHelper
			{
				Application = new Application.ApplicationHelper(helper),
				Client = new Client.ClientHelper(helper),
				Document = new Document.DocumentHelper(helper),
				Folder = new Folder.FolderHelper(helper),
				Workspace = new Workspace.WorkspaceHelper(helper),
				Group = new Group.GroupHelper(helper),
				User = new User.UserHelper(helper),
				Matter = new Matter.MatterHelper(helper),
				RDO = new RDO.RDOHelper(helper)
			};

			return objectHelper;
		}
	}
}