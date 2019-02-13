namespace Relativity.Test.Helpers.Objects
{
	public static class ObjectFactory
	{

		public static ObjectHelper ObjectHelperInstance()
		{
			var objectHelper = new ObjectHelper
			{
				Application = new Application.ApplicationHelper(),
				Client = new Client.ClientHelper(),
				Document = new Document.DocumentHelper(),
				Folder = new Folder.FolderHelper(),
				Workspace = new Workspace.WorkspaceHelper()
			};

			return objectHelper;
		}

	}
}
