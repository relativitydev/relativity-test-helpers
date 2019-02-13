using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Relativity.Test.Helpers.Application;
using Relativity.Test.Helpers.Client;
using Relativity.Test.Helpers.Document;
using Relativity.Test.Helpers.Folder;
using Relativity.Test.Helpers.Workspace;

namespace Relativity.Test.Helpers.Objects
{
	public class ObjectHelper
	{
		public ApplicationHelper Application { get; set; }
		public ClientHelper Client { get; set; }
		public DocumentHelper Document { get; set; }
		public FolderHelper Folder { get; set; }
		public WorkspaceHelper Workspace { get; set; }
	}
}
